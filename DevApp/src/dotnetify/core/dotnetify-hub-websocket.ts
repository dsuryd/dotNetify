/* 
Copyright 2019-2023 Dicky Suryadi

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */
import dotnetify from "..";
import { createEventEmitter } from "../libs/utils";
import { HubOptionsType, IDotnetifyHub, RequestHandlerType } from "../_typings";

export class DotNetifyHubWebSocket implements IDotnetifyHub {
  url = "";
  mode = "websocket";
  debug = false;
  isConnected = false;
  isHubStarted = false;

  responseEvent = createEventEmitter();
  reconnectedEvent = createEventEmitter();
  connectedEvent = createEventEmitter();
  connectionFailedEvent = createEventEmitter();

  reconnectDelay: number[] = [2, 5, 10];
  reconnectRetry: number | null = null;

  _socket: WebSocket = null;
  _vmArgs = {};
  _reconnectCount = 0;
  _reconnectTimeout = null;
  _connectionState = 0;
  _disconnectedHandler = () => {};
  _stateChangedHandler = (state: string) => {};

  constructor(iUrl: string) {
    this.url = iUrl;
  }

  static create(iUrl: string): IDotnetifyHub {
    return new DotNetifyHubWebSocket(iUrl);
  }

  init() {}

  disconnected(iHandler: () => void) {
    this._disconnectedHandler = iHandler;
  }

  stateChanged(iHandler: (state: string) => void) {
    this._stateChangedHandler = iHandler;
  }

  startHub(_: HubOptionsType, doneHandler: () => void, failHandler: (ex: any) => void, iForceRestart: boolean) {
    if (this._socket == null || iForceRestart === true) {
      try {
        this._socket = new WebSocket(this.url);

        this._socket.addEventListener("open", _ => {
          this._changeState(1);
          this._cancelReconnect();
          this.connectedEvent.emit();
          doneHandler();
        });

        this._socket.addEventListener("error", event => {
          this._onDisconnected();
          this.connectionFailedEvent.emit();
          failHandler({ type: "DotNetifyHubException", message: "websocket error attempting to connect to " + this.url });
        });

        this._socket.addEventListener("close", event => {
          if (dotnetify.debug) console.log("DotNetifyHub: websocket close");
          this._onDisconnected();
        });

        this._socket.addEventListener("message", event => {
          try {
            const message: any = JSON.parse(event.data);
            if (message.type === "Response_VM" && message.VMId) this.responseVM(message.VMId, message.VMData);
          } catch (e) {
            if (dotnetify.debug) console.log("DotNetifyHub: not a JSON string", event.data);
          }
        });
      } catch (e) {
        console.error(e);

        failHandler(e);
        this.connectionFailedEvent.emit();
        this._onDisconnected();
      }
    }
  }

  reconnect() {
    // Only attempt reconnect if the specified retry hasn't been exceeded.
    if (this.reconnectRetry == null || this._reconnectCount < this.reconnectRetry) {
      // Determine reconnect delay from the specified configuration array.
      let delay =
        this._reconnectCount < this.reconnectDelay.length
          ? this.reconnectDelay[this._reconnectCount]
          : this.reconnectDelay[this.reconnectDelay.length - 1];

      this._reconnectCount++;

      this._reconnectTimeout = setTimeout(
        function () {
          this._changeState(2);
          this.reconnectedEvent.emit();
        }.bind(this),
        delay * 1000
      );
    } else this._changeState(99);
  }

  _cancelReconnect() {
    if (this._reconnectTimeout) {
      clearTimeout(this._reconnectTimeout);
      this._reconnectTimeout = 0;
    }
  }

  _changeState(iNewState: number) {
    if (this._connectionState === iNewState) return;

    const stateText = {
      0: "connecting",
      1: "connected",
      2: "reconnecting",
      4: "disconnected",
      99: "terminated"
    };

    this.isConnected = iNewState === 1;
    if (iNewState == 1) this._reconnectCount = 0;

    this._stateChangedHandler(stateText[iNewState]);
    this._connectionState = iNewState;
    if (dotnetify.debug) console.log("DotNetifyHub: " + stateText[iNewState]);
  }

  _onDisconnected() {
    this._socket = null;
    this._changeState(4);
    this._disconnectedHandler();
    this.reconnect();
  }

  requestVM(iVMId: string, iVMArgs: any) {
    const data = {
      type: "Request_VM",
      vmId: iVMId,
      vmArgs: iVMArgs
    };
    this._socket.send(JSON.stringify(data));
  }

  updateVM(iVMId: string, iValue: any) {
    const data = {
      type: "Update_VM",
      vmId: iVMId,
      value: iValue
    };
    this._socket.send(JSON.stringify(data));
  }

  disposeVM(iVMId: string) {
    const data = {
      type: "Dispose_VM",
      vmId: iVMId
    };
    this._socket.send(JSON.stringify(data));
  }

  responseVM(iVMId: string, iVMData: any) {
    this.responseEvent.emit(iVMId, iVMData);
  }
}

const createWebSocketHub = DotNetifyHubWebSocket.create;
export { createWebSocketHub };

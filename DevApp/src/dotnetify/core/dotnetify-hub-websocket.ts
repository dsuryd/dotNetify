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
import dotnetify from "./dotnetify";
import { createEventEmitter } from "../libs/utils";
import { HubOptionsType, IDotnetifyHub } from "../_typings";

export class DotNetifyHubWebSocket implements IDotnetifyHub {
  url = "";
  mode = "websocket";
  debug = false;
  isConnected = false;

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

  get isHubStarted(): boolean {
    return !!this._socket;
  }

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
    const done = () => {
      doneHandler && doneHandler();
      this.connectedEvent.emit();
    };
    const fail = (ex: any) => {
      failHandler && failHandler(ex);
      this.connectionFailedEvent.emit();
      throw ex;
    };

    if (this._socket == null || iForceRestart === true) {
      try {
        this._socket = new WebSocket(this.url);

        if (dotnetify.debug) console.log("DotNetifyHub: connecting to " + this.url);

        this._socket.addEventListener("open", _ => {
          this._changeState(1);
          done();
        });

        this._socket.addEventListener("error", _ => {
          this._onDisconnected();
          fail({ type: "DotNetifyHubException", message: "websocket error attempting to connect to " + this.url });
        });

        this._socket.addEventListener("close", event => {
          if (dotnetify.debug) console.log("DotNetifyHub: websocket close");
          this._onDisconnected();
        });

        this._socket.addEventListener("message", event => {
          if (event.data == 404) {
            console.error("The WebSocket server cannot reached the DotNetify server's integration endpoint.");
          } else if (event.data) {
            if (dotnetify.debug) console.debug("ws message:", event.data);
            let jsonData;
            try {
              jsonData = typeof event.data === "string" ? JSON.parse(event.data) : event.data;
            } catch (e) {
              if (dotnetify.debug) console.log("DotNetifyHub: not a valid JSON", event.data);
            }
            const { callType, vmId, data } = jsonData;
            if (callType === "response_vm" && vmId && data) {
              this.responseVM(vmId, data);
            }
          }
        });
      } catch (e) {
        console.error(e);
        fail(e);
        this._onDisconnected();
      }
    } else if (this.isConnected) {
      Promise.resolve().then(() => done());
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
          if (!this.isConnected) {
            this._changeState(2);
            this.reconnectedEvent.emit();
          }
        }.bind(this),
        delay * 1000
      );
    } else this._changeState(99);
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
      callType: "request_vm",
      vmId: iVMId,
      vmArgs: JSON.stringify(iVMArgs)
    };
    this._socket.send(JSON.stringify(data));
  }

  updateVM(iVMId: string, iValue: any) {
    const data = {
      callType: "update_vm",
      vmId: iVMId,
      value: JSON.stringify(iValue)
    };
    this._socket.send(JSON.stringify(data));
  }

  disposeVM(iVMId: string) {
    const data = {
      callType: "dispose_vm",
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

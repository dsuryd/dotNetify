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
          doneHandler();
          this._changeState(1);
          this.connectedEvent.emit();
        });

        this._socket.addEventListener("error", event => {
          failHandler({ type: "DotNetifyHubException", message: "websocket error attempting to connect to " + this.url });
          this._onDisconnected();
          this.connectionFailedEvent.emit();
        });

        this._socket.addEventListener("close", event => {
          if (dotnetify.debug) console.log("DotNetifyHub: websocket close");
          this._onDisconnected();
        });

        this._socket.addEventListener("message", event => {
          console.log(event);
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

      setTimeout(
        function () {
          this._changeState(2);
          this.reconnectedEvent.emit();
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
    // const vmArgs = iVMArgs || {};
    // const vmArgQuery = vmArgs.$vmArg ? "?vmarg=" + JSON.stringify(vmArgs.$vmArg) : "";
    // const headers = vmArgs.$headers || {};
    // this._vmArgs[iVMId] = vmArgs;
    // const url = this.url + `/api/dotnetify/vm/${iVMId}${vmArgQuery}`;
    // fetch("GET", url, null, request => {
    //   Object.keys(headers).forEach(key => request.setRequestHeader(key, headers[key]));
    //   if (typeof this.onRequest == "function") this.onRequest(url, request);
    // })
    //   .then(response => {
    //     if (!response) response = "{}";
    //     this.responseEvent.emit(iVMId, response);
    //   })
    //   .catch(request => console.error(`[${iVMId}] Request failed`, request));
  }

  updateVM(iVMId: string, iValue: any) {
    // const vmArgs = this._vmArgs[iVMId] || {};
    // const vmArgQuery = vmArgs.$vmArg ? "?vmarg=" + JSON.stringify(vmArgs.$vmArg) : "";
    // const headers = vmArgs.$headers || {};
    // const payload = typeof iValue == "object" ? JSON.stringify(iValue) : iValue;
    // const url = this.url + `/api/dotnetify/vm/${iVMId}${vmArgQuery}`;
    // fetch("POST", url, payload, request => {
    //   request.setRequestHeader("Content-Type", "application/json");
    //   Object.keys(headers).forEach(key => request.setRequestHeader(key, headers[key]));
    //   if (typeof this.onRequest == "function") this.onRequest(url, request, payload);
    // })
    //   .then(response => {
    //     if (!response) response = "{}";
    //     this.responseEvent.emit(iVMId, response);
    //   })
    //   .catch(request => console.error(`[${iVMId}] Update failed`, request));
  }

  disposeVM(iVMId: string) {
    // delete this._vmArgs[iVMId];
    // const url = this.url + `/api/dotnetify/vm/${iVMId}`;
    // fetch("DELETE", url, null, request => {
    //   if (typeof this.onRequest == "function") this.onRequest(url, request);
    // }).catch(request => console.error(`[${iVMId}] Dispose failed`, request));
  }
}

const createWebSocketHub = DotNetifyHubWebSocket.create;
export { createWebSocketHub };

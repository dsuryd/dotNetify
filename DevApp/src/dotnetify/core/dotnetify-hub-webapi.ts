/* 
Copyright 2019-2020 Dicky Suryadi

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
import { createEventEmitter, fetch } from "../libs/utils";
import { IDotnetifyHub, RequestHandlerType } from "../_typings";

export class DotNetifyHubWebApi implements IDotnetifyHub {
  url = "";
  mode = "webapi";
  debug = false;
  isConnected = false;
  isHubStarted = false;

  responseEvent = createEventEmitter();
  reconnectedEvent = createEventEmitter();
  connectedEvent = createEventEmitter();
  connectionFailedEvent = createEventEmitter();

  reconnectDelay = [];
  reconnectRetry: 0;

  onRequest: RequestHandlerType;

  _vmArgs = {};

  constructor(iBaseUrl: string, iOnRequest: RequestHandlerType) {
    this.url = iBaseUrl || "";
    this.onRequest = iOnRequest;
  }

  static create(iBaseUrl?: string, iOnRequest?: RequestHandlerType): IDotnetifyHub {
    return new DotNetifyHubWebApi(iBaseUrl, iOnRequest);
  }

  init() {}
  stateChanged() {}

  startHub() {
    this.isConnected = true;
    this.isHubStarted = true;
    this.connectedEvent.emit();
  }

  requestVM(iVMId: string, iVMArgs: any) {
    const vmArgs = iVMArgs || {};
    const vmArgQuery = vmArgs.$vmArg ? "?vmarg=" + JSON.stringify(vmArgs.$vmArg) : "";
    const headers = vmArgs.$headers || {};

    this._vmArgs[iVMId] = vmArgs;
    const url = this.url + `/api/dotnetify/vm/${iVMId}${vmArgQuery}`;

    fetch("GET", url, null, (request) => {
      Object.keys(headers).forEach((key) => request.setRequestHeader(key, headers[key]));
      if (typeof this.onRequest == "function") this.onRequest(url, request);
    })
      .then((response) => {
        this.responseEvent.emit(iVMId, response);
      })
      .catch((request) => console.error(`[${iVMId}] Request failed`, request));
  }

  updateVM(iVMId: string, iValue: any) {
    const vmArgs = this._vmArgs[iVMId] || {};
    const vmArgQuery = vmArgs.$vmArg ? "?vmarg=" + JSON.stringify(vmArgs.$vmArg) : "";
    const headers = vmArgs.$headers || {};
    const payload = typeof iValue == "object" ? JSON.stringify(iValue) : iValue;

    const url = this.url + `/api/dotnetify/vm/${iVMId}${vmArgQuery}`;

    fetch("POST", url, payload, (request) => {
      request.setRequestHeader("Content-Type", "application/json");
      Object.keys(headers).forEach((key) => request.setRequestHeader(key, headers[key]));
      if (typeof this.onRequest == "function") this.onRequest(url, request, payload);
    })
      .then((response) => {
        this.responseEvent.emit(iVMId, response);
      })
      .catch((request) => console.error(`[${iVMId}] Update failed`, request));
  }

  disposeVM(iVMId) {
    delete this._vmArgs[iVMId];
  }
}

const createWebApiHub = DotNetifyHubWebApi.create;

export { createWebApiHub };
export default DotNetifyHubWebApi.create();

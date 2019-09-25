/* 
Copyright 2019 Dicky Suryadi

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
import { createEventEmitter, fetch } from '../libs/utils';

export class dotNetifyHubWebApi {
  mode = 'webapi';
  debug = false;
  isConnected = false;
  isHubStarted = false;

  responseEvent = createEventEmitter();
  reconnectedEvent = createEventEmitter();
  connectedEvent = createEventEmitter();
  connectionFailedEvent = createEventEmitter();

  _vmArgs = {};

  constructor(iBaseUrl, iOnRequest) {
    this.baseUrl = iBaseUrl || '';
    this.onRequest = iOnRequest;
  }

  static create(iBaseUrl, iOnRequest) {
    return new dotNetifyHubWebApi(iBaseUrl, iOnRequest);
  }

  startHub() {
    this.isConnected = true;
    this.isHubStarted = true;
    this.connectedEvent.emit();
  }

  requestVM(iVMId, iVMArgs) {
    const vmArgs = iVMArgs || {};
    const vmArgQuery = vmArgs.$vmArg ? '?vmarg=' + JSON.stringify(vmArgs.$vmArg) : '';
    const headers = vmArgs.$headers || {};

    this._vmArgs[iVMId] = vmArgs;
    const url = this.baseUrl + `/api/dotnetify/vm/${iVMId}${vmArgQuery}`;

    fetch('GET', url, null, request => {
      Object.keys(headers).forEach(key => request.setRequestHeader(key, headers[key]));
      if (typeof this.onRequest == 'function') this.onRequest(url, request);
    })
      .then(response => {
        this.responseEvent.emit(iVMId, response);
      })
      .catch(request => console.error(`[${iVMId}] Request failed`, request));
  }

  updateVM(iVMId, iValue) {
    const vmArgs = this._vmArgs[iVMId] || {};
    const vmArgQuery = vmArgs.$vmArg ? '?vmarg=' + JSON.stringify(vmArgs.$vmArg) : '';
    const headers = vmArgs.$headers || {};
    const payload = typeof iValue == 'object' ? JSON.stringify(iValue) : iValue;

    const url = this.baseUrl + `/api/dotnetify/vm/${iVMId}${vmArgQuery}`;

    fetch('POST', url, payload, request => {
      request.setRequestHeader('Content-Type', 'application/json');
      Object.keys(headers).forEach(key => request.setRequestHeader(key, headers[key]));
      if (typeof this.onRequest == 'function') this.onRequest(url, request, payload);
    })
      .then(response => {
        this.responseEvent.emit(iVMId, response);
      })
      .catch(request => console.error(`[${iVMId}] Update failed`, request));
  }

  disposeVM(iVMId) {
    delete this._vmArgs[iVMId];
  }
}

const createWebApiHub = dotNetifyHubWebApi.create;

export { createWebApiHub };
export default dotNetifyHubWebApi.create();

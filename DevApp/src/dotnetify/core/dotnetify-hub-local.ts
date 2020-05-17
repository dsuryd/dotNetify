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
import { createEventEmitter } from "../libs/utils";
import { IDotnetifyHub } from "./dotnetify-hub";

const _window = window || global || {};

const normalize = (iVMId) => iVMId && iVMId.replace(/\./g, "_");
const hasLocalVM = (iVMId) => {
  const vmId = normalize(iVMId);
  const vm = _window[vmId];
  return typeof vm == "object" && typeof vm.onConnect == "function";
};

export class dotNetifyHubLocal implements IDotnetifyHub {
  url = null;
  mode = "local";
  debug = false;
  isConnected = false;
  isHubStarted = false;

  responseEvent = createEventEmitter();
  reconnectedEvent = createEventEmitter();
  connectedEvent = createEventEmitter();
  connectionFailedEvent = createEventEmitter();

  reconnectDelay = [];
  reconnectRetry: 0;

  init() {}
  stateChanged() {}

  startHub() {
    this.isConnected = true;
    this.isHubStarted = true;
    this.connectedEvent.emit();
  }

  requestVM(iVMId: string, iVMArgs: any) {
    const vmId = normalize(iVMId);
    const vm = _window[vmId];

    if (typeof vm === "object" && typeof vm.onConnect == "function") {
      if (this.debug) console.log(`[${iVMId}] *** local mode ***`);

      vm.$pushUpdate = (update) => {
        if (typeof update == "object") update = JSON.stringify(update);
        setTimeout(() => this.responseEvent.emit(iVMId, update));
      };

      vm.$pushUpdate(vm.onConnect(iVMArgs) || {});
    }
  }

  updateVM(iVMId: string, iValue: any) {
    const vmId = normalize(iVMId);
    const vm = _window[vmId];

    if (typeof vm === "object" && typeof vm.onDispatch == "function") {
      let state = vm.onDispatch(iValue);
      if (state) {
        if (typeof state == "object") state = JSON.stringify(state);
        setTimeout(() => this.responseEvent.emit(iVMId, state));
      }
    }
  }

  disposeVM(iVMId: string) {
    const vmId = normalize(iVMId);
    const vm = _window[vmId];

    if (typeof vm === "object" && typeof vm.onDestroy == "function") {
      vm.onDestroy(iVMId);
    }
  }
}

export default new dotNetifyHubLocal();
export { hasLocalVM };

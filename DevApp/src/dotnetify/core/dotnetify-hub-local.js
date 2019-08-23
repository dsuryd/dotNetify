/* 
import { dotnetify } from 'dotnetify/dist/dotnetify-ko';
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
import { createEventEmitter } from '../libs/utils';

export class dotNetifyHubLocal {
  mode = 'local';
  isConnected = false;
  isHubStarted = false;

  responseEvent = createEventEmitter();
  reconnectedEvent = createEventEmitter();
  connectedEvent = createEventEmitter();
  connectionFailedEvent = createEventEmitter();

  constructor(iDotnetify, iVMConnectArgs) {
    this.dotnetify = iDotnetify;
    this.vmId = iVMConnectArgs.vmId.replace(/\./g, '_');
    this.initialState = iVMConnectArgs.options.initialState;
    this.onDispatch = val => iVMConnectArgs.options.onDispatch(val);
  }

  startHub() {
    this.isConnected = true;
    this.isHubStarted = true;
    this.connectedEvent.emit();
  }

  requestVM(iVMId) {
    const update = vmData => {
      const vm = dotNetify.getViewModels().find(x => x.$vmId === iVMId);
      vm && vm.$update(typeof vmData === 'object' ? JSON.stringify(vmData) : vmData);
    };

    // Local view model.
    if (typeof window[this.vmId] === 'object') {
      this.initialState = this.initialState || window[this.vmId].initialState;
      this.onDispatch = this.onDispatch || window[this.vmId].onDispatch;
      window[this.vmId].$update = update;
    }

    setTimeout(() => update(this.initialState));
  }

  updateVM(iVMId, iValue) {
    this.onDispatch(iValue);
  }

  disposeVM() {}
}

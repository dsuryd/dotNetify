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

export class dotNetifyHubLocal {
  // iDotNetify - framework-specific dotNetify library.
  // vmConnectArgs - View model connect arguments.
  constructor(iDotNetify, vmConnectArgs) {
    super();

    this.dotNetify = iDotNetify;
    this.vmId = vmConnectArgs.vmId.replace(/\./g, '_');
    this.initialState = vmConnectArgs.options.initialState;
    this.onDispatch = vmConnectArgs.options.onDispatch;

    if (typeof window[this.vmId] === 'object') {
      this.initialState = this.initialState || window[this.vmId].initialState;
      this.onDispatch = this.onDispatch || window[this.vmId].onDispatch;
    }
  }

  get mode() {
    return 'local';
  }

  get isConnected() {
    return true;
  }

  get isHubStarted() {
    return true;
  }

  requestVM(iVMId, iArgs) {
    const update = vmData =>
      this.dotNetify.viewModels[iVMId].$update(typeof vmData === 'object' ? JSON.stringify(vmData) : vmData);

    window[this.vmId].$update = update;
    setTimeout(() => update(this.initialState));
  }

  updateVM(iVMId, iValue) {
    this.onDispatch(iVMId, iValue);
  }

  disposeVM(iVMId) {}
}

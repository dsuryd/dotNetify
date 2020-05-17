/* 
Copyright 2017-2018 Dicky Suryadi

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
import * as React from "react";
import _dotnetify, { IDotnetifyImpl, Dotnetify, ConnectOptionsType } from "../core/dotnetify";
import DotnetifyVM, { IDotnetifyVM } from "../core/dotnetify-vm";
import { IDotnetifyHub } from "../core/dotnetify-hub";

const _window = window || global || <any>{};
let dotnetify: Dotnetify = _window.dotnetify || _dotnetify;

export interface IDotnetifyReact {
  connect(iVMId: string, iReact: React.Component | any, iOptions?: ConnectOptionsType): IDotnetifyVM;
}

export class DotnetifyReact implements IDotnetifyReact, IDotnetifyImpl {
  version = "3.0.0";
  viewModels: { [vmId: string]: DotnetifyVM } = {};
  plugins: { [pluginId: string]: any } = {};
  controller = dotnetify;

  // Internal variables.
  _hubs = [];

  // Initializes connection to SignalR server hub.
  init(iHub: IDotnetifyHub) {
    const hubInitialized = this._hubs.some((hub) => hub === iHub);

    const start = () => {
      if (!iHub.isHubStarted)
        Object.keys(this.viewModels)
          .filter((vmId) => this.viewModels[vmId].$hub === iHub)
          .forEach((vmId) => (this.viewModels[vmId].$requested = false));

      dotnetify.startHub(iHub);
    };

    if (!hubInitialized) {
      iHub.responseEvent.subscribe((iVMId, iVMData) => this._responseVM(iVMId, iVMData));
      iHub.connectedEvent.subscribe(() =>
        Object.keys(this.viewModels)
          .filter((vmId) => this.viewModels[vmId].$hub === iHub && !this.viewModels[vmId].$requested)
          .forEach((vmId) => this.viewModels[vmId].$request())
      );
      iHub.reconnectedEvent.subscribe(start);

      if (iHub.mode !== "local") this._hubs.push(iHub);
    }

    start();
  }

  // Connects to a server view model.
  connect(iVMId: string, iReact: React.Component | any, iOptions: ConnectOptionsType): IDotnetifyVM {
    const self = dotnetify.react;
    if (self.viewModels.hasOwnProperty(iVMId)) {
      console.error(
        `Component is attempting to connect to an already active '${iVMId}'. ` +
          ` If it's from a dismounted component, you must add vm.$destroy to componentWillUnmount().`
      );
      self.viewModels[iVMId].$destroy();
    }

    const component = {
      get props() {
        return iReact.props;
      },
      get state() {
        return iReact.state;
      },
      setState(state: any) {
        iReact.setState(state);
      },
    };

    const connectInfo = dotnetify.selectHub({ vmId: iVMId, options: iOptions, hub: null });
    self.viewModels[iVMId] = new DotnetifyVM(connectInfo.vmId, component, connectInfo.options, self, connectInfo.hub);
    if (connectInfo.hub) self.init(connectInfo.hub);

    return self.viewModels[iVMId];
  }

  // Get all view models.
  getViewModels(): IDotnetifyVM[] {
    const self = dotnetify.react;
    return Object.keys(self.viewModels).map((vmId) => self.viewModels[vmId]);
  }

  _responseVM(iVMId: string, iVMData: any): boolean {
    const self = dotnetify.react;

    if (self.viewModels.hasOwnProperty(iVMId)) {
      const vm = self.viewModels[iVMId];
      dotnetify.checkServerSideException(iVMId, iVMData, vm.$exceptionHandler);
      vm.$update(iVMData);
      return true;
    }
    return false;
  }
}

dotnetify.react = new DotnetifyReact();
dotnetify.addVMAccessor(dotnetify.react.getViewModels);

export default dotnetify;

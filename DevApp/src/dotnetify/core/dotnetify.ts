/* 
Copyright 2017-2020 Dicky Suryadi

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

import dotnetifyHub, { dotnetifyHubFactory } from "./dotnetify-hub";
import localHub, { hasLocalVM } from "./dotnetify-hub-local";
import webApiHub, { createWebApiHub } from "./dotnetify-hub-webapi";
import DotnetifyVM from "./dotnetify-vm";
import {
  IDotnetify,
  IDotnetifyHub,
  IDotnetifyVM,
  IConnectOptions,
  VMConnectArgsType,
  ExceptionType,
  ExceptionHandlerType,
  RequestHandlerType
} from "../_typings";

export interface IDotnetifyImpl {
  // Core instance.
  controller: Dotnetify;

  // Active view models.
  viewModels: { [vmId: string]: DotnetifyVM };

  // Plugins such as router.
  plugins: { [pluginId: string]: any };

  // Initializes connection to SignalR server hub.
  init: (iHub: IDotnetifyHub) => void;

  // Active view models.
  getViewModels(): IDotnetifyVM[];
}

export type VMAccessorsType = () => IDotnetifyVM[];

export class Dotnetify implements IDotnetify {
  // Supported JS framework.
  react = null;
  vue = null;
  ko = null;

  // SignalR hub options.
  hub = dotnetifyHub;
  hubOptions = {
    transport: ["webSockets", "longPolling"],

    // Use this to add customize HubConnectionBuilder.
    connectionBuilder: (builder: any) => builder
  };
  hubPath = null;
  hubLib = null;

  // Debug mode.
  debug = false;
  debugFn = null;

  // Use this to get notified of connection state changed events.
  connectionStateHandler = null;

  // Use this intercept a view model prior to establishing connection.
  connectHandler = null;

  // Internal variables.
  _vmAccessors = [];

  // Support changing hub server URL after first init.
  get hubServerUrl() {
    return this.hub.url;
  }

  set hubServerUrl(iUrl: string) {
    this.hub.url = iUrl;
    if (this.debug) console.log("SignalR: connecting to " + this.hubServerUrl);
    if (this.hub.isHubStarted) this.startHub(this.hub, true);
  }

  // Generic connect function for non-React app.
  connect(iVMId: string, iOptions: IConnectOptions) {
    return _dotnetify.react.connect(iVMId, {}, iOptions);
  }

  // Creates a SignalR hub client.
  createHub(iHubServerUrl: string, iHubPath: string, iHubLib: any) {
    return this.initHub(dotnetifyHubFactory.create(), iHubPath, iHubServerUrl, iHubLib);
  }

  // Creates a Web API hub client.
  createWebApiHub(iBaseUrl: string, iRequestHandler: RequestHandlerType): IDotnetifyHub {
    return createWebApiHub(iBaseUrl, iRequestHandler);
  }

  // Configures hub connection to SignalR hub server.
  initHub(iHub?: IDotnetifyHub, iHubPath?: string, iHubServerUrl?: string, iHubLib?: any) {
    const hub = iHub || this.hub;
    const hubPath = iHubPath || this.hubPath;
    const hubServerUrl = iHubServerUrl || this.hubServerUrl;
    const hubLib = iHubLib || this.hubLib;

    if (!hub.isHubStarted) {
      hub.init(hubPath, hubServerUrl, hubLib);

      // Use SignalR event to raise the connection state event.
      hub.stateChanged((state: string) => this.handleConnectionStateChanged(state, null, hub));
    }
    return hub;
  }

  // Used by a view to select a hub, and provides the opportunity to override any connect info.
  selectHub(vmConnectArgs: VMConnectArgsType): VMConnectArgsType {
    vmConnectArgs = vmConnectArgs || <VMConnectArgsType>{};
    vmConnectArgs.options = vmConnectArgs.options || {};
    let override = (typeof this.connectHandler == "function" && this.connectHandler(vmConnectArgs)) || {};
    if (!override.hub) {
      override.hub = hasLocalVM(vmConnectArgs.vmId) ? localHub : vmConnectArgs.options.webApi ? webApiHub : this.initHub();
      override.hub.debug = this.debug;
    }
    return Object.assign(vmConnectArgs, override);
  }

  // Starts hub connection to SignalR hub server.
  startHub(iHub: IDotnetifyHub, iForceRestart?: boolean) {
    const hub = iHub || this.hub;

    const doneHandler = () => {};
    const failHandler = (ex: any) => this.handleConnectionStateChanged("error", ex, hub);
    hub.startHub(this.hubOptions, doneHandler, failHandler, iForceRestart);
  }

  // Used by framework-specific dotnetify instances to expose their view model accessors.
  addVMAccessor(iVMAccessor: VMAccessorsType) {
    !this._vmAccessors.includes(iVMAccessor) && this._vmAccessors.push(iVMAccessor);
  }

  // Get all view models.
  getViewModels(): IDotnetifyVM[] {
    return this._vmAccessors
      .reduce((prev: IDotnetifyVM[], current: VMAccessorsType) => [...prev, ...current()], [])
      .filter((val: IDotnetifyVM, idx: number, self: IDotnetifyVM[]) => self.indexOf(val) === idx); // returns distinct items.
  }

  handleConnectionStateChanged(iState: string, iException: ExceptionType, iHub: IDotnetifyHub) {
    if (this.debug) console.log("SignalR: " + (iException ? iException.message : iState));
    if (typeof this.connectionStateHandler === "function") this.connectionStateHandler(iState, iException, iHub);
    else if (iException) console.error(iException);
  }

  checkServerSideException(iVMId: string, iVMData: any, iExceptionHandler: ExceptionHandlerType) {
    if (!iVMData) return;
    const vmData = JSON.parse(iVMData);
    if (vmData && vmData.hasOwnProperty("ExceptionType") && vmData.hasOwnProperty("Message")) {
      const exception: ExceptionType = {
        name: vmData.ExceptionType,
        message: vmData.Message
      };

      if (typeof iExceptionHandler === "function") {
        iExceptionHandler(exception);
      } else {
        console.error("[" + iVMId + "] " + exception.name + ": " + exception.message);
        throw exception;
      }
    }
  }
}

const _dotnetify = new Dotnetify();
export default _dotnetify;

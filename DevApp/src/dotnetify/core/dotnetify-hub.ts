﻿/* 
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
import { createEventEmitter } from "../libs/utils";
import * as jQueryShim from "../libs/jquery-shim";
import * as signalRNetCore from "@microsoft/signalr";
import { IDotnetifyHub } from "../_typings";

let $ = jQueryShim;
const _window = window || global || <any>{};

export class dotnetifyHubFactory {
  static create(): IDotnetifyHub {
    let dotnetifyHub: any = {
      version: "2.0.0",
      type: null,

      reconnectDelay: [2, 5, 10],
      reconnectRetry: null,

      _startInfo: null,
      _init: false,

      // Hub server methods.
      requestVM: (iVMId, iOptions) => dotnetifyHub.server.request_VM(iVMId, iOptions),
      updateVM: (iVMId, iValue) => dotnetifyHub.server.update_VM(iVMId, iValue),
      disposeVM: iVMId => dotnetifyHub.server.dispose_VM(iVMId),

      // Connection events.
      responseEvent: createEventEmitter(),
      reconnectedEvent: createEventEmitter(),
      connectedEvent: createEventEmitter(),
      connectionFailedEvent: createEventEmitter(),

      get isHubStarted(): boolean {
        return !!this._startInfo;
      },

      // Starts connection with SignalR hub server.
      startHub(hubOptions, doneHandler, failHandler, forceRestart) {
        const _doneHandler = () => {
          if (typeof doneHandler == "function") doneHandler();
          this.connectedEvent.emit();
        };
        const _failHandler = ex => {
          if (typeof failHandler == "function") failHandler();
          this.connectionFailedEvent.emit();
          throw ex;
        };

        if (this._startInfo === null || forceRestart) {
          try {
            this._startInfo = this.start(hubOptions).done(_doneHandler).fail(_failHandler);
          } catch (err) {
            this._startInfo = null;
          }
        } else {
          try {
            this._startInfo.done(_doneHandler);
          } catch (err) {
            this._startInfo = null;
            return this.startHub(hubOptions, doneHandler, failHandler, forceRestart);
          }
        }
      }
    };

    // Configures connection to SignalR hub server.
    dotnetifyHub.init = function (iHubPath, iServerUrl, signalR) {
      if (dotnetifyHub._init) return;

      dotnetifyHub._init = true;
      signalR = signalR || _window.signalR || signalRNetCore;

      // SignalR .NET Core.
      if (signalR && signalR.HubConnection) {
        dotnetifyHub.type = "netcore";

        Object.defineProperty(dotnetifyHub, "isConnected", {
          get: function () {
            return dotnetifyHub._connection && dotnetifyHub._connection.connection.connectionState === signalR.HubConnectionState.Connected;
          }
        });

        dotnetifyHub = $.extend(dotnetifyHub, {
          hubPath: iHubPath || "/dotnetify",
          url: iServerUrl,

          // Internal variables. Do not modify!
          _connection: null,
          _reconnectCount: 0,
          _startDoneHandler: null,
          _startFailHandler: null,
          _disconnectedHandler: function () {},
          _stateChangedHandler: function (iNewState) {},

          _onDisconnected: function () {
            dotnetifyHub._changeState(4);
            dotnetifyHub._disconnectedHandler();
          },

          _changeState: function (iNewState) {
            if (iNewState == 1) dotnetifyHub._reconnectCount = 0;

            let stateText = {
              0: "connecting",
              1: "connected",
              2: "reconnecting",
              4: "disconnected",
              99: "terminated"
            };
            dotnetifyHub._stateChangedHandler(stateText[iNewState]);
          },

          _startConnection: function (iHubOptions, iTransportArray) {
            let url = dotnetifyHub.url ? dotnetifyHub.url + dotnetifyHub.hubPath : dotnetifyHub.hubPath;
            let hubOptions: any = {};
            Object.keys(iHubOptions).forEach(function (key) {
              hubOptions[key] = iHubOptions[key];
            });
            hubOptions.transport = iTransportArray.shift();

            let hubConnectionBuilder = new signalR.HubConnectionBuilder().withUrl(url, hubOptions);
            if (typeof hubOptions.connectionBuilder == "function")
              hubConnectionBuilder = hubOptions.connectionBuilder(hubConnectionBuilder);

            dotnetifyHub._connection = hubConnectionBuilder.build();
            dotnetifyHub._connection.on("response_vm", dotnetifyHub.client.response_VM);
            dotnetifyHub._connection.onclose(dotnetifyHub._onDisconnected);

            let promise = dotnetifyHub._connection
              .start()
              .then(function () {
                dotnetifyHub._changeState(1);
              })
              .catch(function () {
                // If failed to start, fallback to the next transport.
                if (iTransportArray.length > 0) dotnetifyHub._startConnection(iHubOptions, iTransportArray);
                else dotnetifyHub._onDisconnected();
              });

            if (typeof dotnetifyHub._startDoneHandler === "function")
              promise.then(dotnetifyHub._startDoneHandler).catch(dotnetifyHub._startFailHandler || function () {});
            return promise;
          },

          start: function (iHubOptions) {
            dotnetifyHub._startDoneHandler = null;
            dotnetifyHub._startFailHandler = null;

            // Map the transport option.
            let transport = [0];
            let transportOptions = {
              webSockets: 0,
              serverSentEvents: 1,
              longPolling: 2
            };
            if (iHubOptions && Array.isArray(iHubOptions.transport))
              transport = iHubOptions.transport.map(function (arg) {
                return transportOptions[arg];
              });

            let promise = dotnetifyHub._startConnection(iHubOptions, transport);
            return {
              done: function (iHandler) {
                dotnetifyHub._startDoneHandler = iHandler;
                promise.then(iHandler).catch(function (error) {
                  throw error;
                });
                return this;
              },
              fail: function (iHandler) {
                dotnetifyHub._startFailHandler = iHandler;
                promise.catch(iHandler);
                return this;
              }
            };
          },

          disconnected: function (iHandler) {
            if (typeof iHandler === "function") dotnetifyHub._disconnectedHandler = iHandler;
          },

          stateChanged: function (iHandler) {
            if (typeof iHandler === "function") dotnetifyHub._stateChangedHandler = iHandler;
          },

          reconnect: function (iStartHubFunc) {
            if (typeof iStartHubFunc === "function") {
              // Only attempt reconnect if the specified retry hasn't been exceeded.
              if (!dotnetifyHub.reconnectRetry || dotnetifyHub._reconnectCount < dotnetifyHub.reconnectRetry) {
                // Determine reconnect delay from the specified configuration array.
                let delay =
                  dotnetifyHub._reconnectCount < dotnetifyHub.reconnectDelay.length
                    ? dotnetifyHub.reconnectDelay[dotnetifyHub._reconnectCount]
                    : dotnetifyHub.reconnectDelay[dotnetifyHub.reconnectDelay.length - 1];

                dotnetifyHub._reconnectCount++;

                setTimeout(function () {
                  dotnetifyHub._changeState(2);
                  iStartHubFunc();
                }, delay * 1000);
              } else dotnetifyHub._changeState(99);
            }
          },

          client: {},

          server: {
            dispose_VM: function (iVMId) {
              dotnetifyHub._connection.invoke("Dispose_VM", iVMId);
            },
            update_VM: function (iVMId, iValue) {
              dotnetifyHub._connection.invoke("Update_VM", iVMId, iValue);
            },
            request_VM: function (iVMId, iArgs) {
              dotnetifyHub._connection.invoke("Request_VM", iVMId, iArgs);
            }
          }
        });
      } else {
        // SignalR .NET FX.
        dotnetifyHub.type = "netfx";

        if (_window.jQuery) $ = _window.jQuery;

        // SignalR hub auto-generated from /signalr/hubs.
        /// <reference path="..\..\SignalR.Client.JS\Scripts\jquery-1.6.4.js" />
        /// <reference path="jquery.signalR.js" />
        (function ($) {
          /// <param name="$" type="jQuery" />
          "use strict";

          if (typeof $.signalR !== "function") {
            throw new Error("SignalR: SignalR is not loaded. Please ensure jquery.signalR-x.js is referenced before ~/signalr/js.");
          }

          var signalR = $.signalR;

          function makeProxyCallback(hub, callback) {
            return function () {
              // Call the client hub method
              callback.apply(hub, $.makeArray(arguments));
            };
          }

          function registerHubProxies(instance, shouldSubscribe) {
            var key, hub, memberKey, memberValue, subscriptionMethod;

            for (key in instance) {
              if (instance.hasOwnProperty(key)) {
                hub = instance[key];

                if (!hub.hubName) {
                  // Not a client hub
                  continue;
                }

                if (shouldSubscribe) {
                  // We want to subscribe to the hub events
                  subscriptionMethod = hub.on;
                } else {
                  // We want to unsubscribe from the hub events
                  subscriptionMethod = hub.off;
                }

                // Loop through all members on the hub and find client hub functions to subscribe/unsubscribe
                for (memberKey in hub.client) {
                  if (hub.client.hasOwnProperty(memberKey)) {
                    memberValue = hub.client[memberKey];

                    if (!$.isFunction(memberValue)) {
                      // Not a client hub function
                      continue;
                    }

                    subscriptionMethod.call(hub, memberKey, makeProxyCallback(hub, memberValue));
                  }
                }
              }
            }
          }

          $.hubConnection.prototype.createHubProxies = function () {
            var proxies = {};
            this.starting(function () {
              // Register the hub proxies as subscribed
              // (instance, shouldSubscribe)
              registerHubProxies(proxies, true);

              this._registerSubscribedHubs();
            }).disconnected(function () {
              // Unsubscribe all hub proxies when we "disconnect".  This is to ensure that we do not re-add functional call backs.
              // (instance, shouldSubscribe)
              registerHubProxies(proxies, false);
            });

            proxies["dotNetifyHub"] = this.createHubProxy("dotNetifyHub");
            proxies["dotNetifyHub"].client = {};
            proxies["dotNetifyHub"].server = {
              dispose_VM: function (vmId) {
                return proxies["dotNetifyHub"].invoke.apply(proxies["dotNetifyHub"], $.merge(["Dispose_VM"], $.makeArray(arguments)));
              },

              request_VM: function (vmId, vmArg) {
                return proxies["dotNetifyHub"].invoke.apply(proxies["dotNetifyHub"], $.merge(["Request_VM"], $.makeArray(arguments)));
              },

              update_VM: function (vmId, vmData) {
                return proxies["dotNetifyHub"].invoke.apply(proxies["dotNetifyHub"], $.merge(["Update_VM"], $.makeArray(arguments)));
              }
            };

            return proxies;
          };

          signalR.hub = $.hubConnection(dotnetifyHub.hubPath, {
            useDefaultPath: false
          });
          $.extend(signalR, signalR.hub.createHubProxies());
        })($);

        Object.defineProperty(dotnetifyHub, "state", {
          get: function () {
            return $.connection.hub.state;
          },
          set: function (val) {
            $.connection.hub.state = val;
          }
        });

        Object.defineProperty(dotnetifyHub, "client", {
          get: function () {
            return $.connection.dotNetifyHub.client;
          }
        });

        Object.defineProperty(dotnetifyHub, "server", {
          get: function () {
            return $.connection.dotNetifyHub.server;
          }
        });

        Object.defineProperty(dotnetifyHub, "isConnected", {
          get: function () {
            return $.connection.hub.state == $.signalR.connectionState.connected;
          }
        });

        dotnetifyHub = $.extend(dotnetifyHub, {
          hubPath: iHubPath || "/signalr",
          url: iServerUrl,

          _reconnectCount: 0,
          _stateChangedHandler: function (iNewState) {},

          start: function (iHubOptions) {
            if (dotnetifyHub.url) $.connection.hub.url = dotnetifyHub.url;

            let deferred;
            if (iHubOptions) deferred = $.connection.hub.start(iHubOptions);
            else deferred = $.connection.hub.start();
            deferred.fail(function (error) {
              if (error.source && error.source.message === "Error parsing negotiate response.")
                console.warn("This client may be attempting to connect to an incompatible SignalR .NET Core server.");
            });
            return deferred;
          },

          disconnected: function (iHandler) {
            return $.connection.hub.disconnected(iHandler);
          },

          stateChanged: function (iHandler) {
            dotnetifyHub._stateChangedHandler = iHandler;
            return $.connection.hub.stateChanged(function (state) {
              if (state == 1) dotnetifyHub._reconnectCount = 0;

              let stateText = {
                0: "connecting",
                1: "connected",
                2: "reconnecting",
                4: "disconnected"
              };
              iHandler(stateText[state.newState]);
            });
          },

          reconnect: function (iStartHubFunc) {
            if (typeof iStartHubFunc === "function") {
              // Only attempt reconnect if the specified retry hasn't been exceeded.
              if (!dotnetifyHub.reconnectRetry || dotnetifyHub._reconnectCount < dotnetifyHub.reconnectRetry) {
                // Determine reconnect delay from the specified configuration array.
                let delay =
                  dotnetifyHub._reconnectCount < dotnetifyHub.reconnectDelay.length
                    ? dotnetifyHub.reconnectDelay[dotnetifyHub._reconnectCount]
                    : dotnetifyHub.reconnectDelay[dotnetifyHub.reconnectDelay.length - 1];

                dotnetifyHub._reconnectCount++;

                setTimeout(function () {
                  dotnetifyHub._stateChangedHandler("reconnecting");
                  iStartHubFunc();
                }, delay * 1000);
              } else dotnetifyHub._stateChangedHandler("terminated");
            }
          }
        });
      }

      // Setup SignalR server method handler.
      dotnetifyHub.client.response_VM = function (iVMId, iVMData) {
        // SignalR .NET Core is sending an array of arguments.
        if (Array.isArray(iVMId)) {
          iVMData = iVMId[1];
          iVMId = iVMId[0];
        }

        let handled = dotnetifyHub.responseEvent.emit(iVMId, iVMData);

        // If we get to this point, that means the server holds a view model instance
        // whose view no longer existed.  So, tell the server to dispose the view model.
        if (!handled) dotnetifyHub.server.dispose_VM(iVMId);
      };

      // On disconnected, keep attempting to start the connection.
      dotnetifyHub.disconnected(function () {
        dotnetifyHub._startInfo = null;
        dotnetifyHub.reconnect(function () {
          dotnetifyHub.reconnectedEvent.emit();
        });
      });
    };

    return dotnetifyHub;
  }
}

export default dotnetifyHubFactory.create();

(function webpackUniversalModuleDefinition(root, factory) {
	if(typeof exports === 'object' && typeof module === 'object')
		module.exports = factory(require("@aspnet/signalr"));
	else if(typeof define === 'function' && define.amd)
		define(["@aspnet/signalr"], factory);
	else if(typeof exports === 'object')
		exports["dotnetify"] = factory(require("@aspnet/signalr"));
	else
		root["dotnetify"] = factory(root["signalR"]);
})(window, function(__WEBPACK_EXTERNAL_MODULE__8__) {
return /******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, { enumerable: true, get: getter });
/******/ 		}
/******/ 	};
/******/
/******/ 	// define __esModule on exports
/******/ 	__webpack_require__.r = function(exports) {
/******/ 		if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 			Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 		}
/******/ 		Object.defineProperty(exports, '__esModule', { value: true });
/******/ 	};
/******/
/******/ 	// create a fake namespace object
/******/ 	// mode & 1: value is a module id, require it
/******/ 	// mode & 2: merge all properties of value into the ns
/******/ 	// mode & 4: return value when already ns object
/******/ 	// mode & 8|1: behave like require
/******/ 	__webpack_require__.t = function(value, mode) {
/******/ 		if(mode & 1) value = __webpack_require__(value);
/******/ 		if(mode & 8) return value;
/******/ 		if((mode & 4) && typeof value === 'object' && value && value.__esModule) return value;
/******/ 		var ns = Object.create(null);
/******/ 		__webpack_require__.r(ns);
/******/ 		Object.defineProperty(ns, 'default', { enumerable: true, value: value });
/******/ 		if(mode & 2 && typeof value != 'string') for(var key in value) __webpack_require__.d(ns, key, function(key) { return value[key]; }.bind(null, key));
/******/ 		return ns;
/******/ 	};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = 1);
/******/ })
/************************************************************************/
/******/ ([
/* 0 */
/***/ (function(module, exports) {

var g;

// This works in non-strict mode
g = (function() {
	return this;
})();

try {
	// This works if eval is allowed (see CSP)
	g = g || Function("return this")() || (1, eval)("this");
} catch (e) {
	// This works if the window reference is available
	if (typeof window === "object") g = window;
}

// g can still be undefined, but nothing to do about it...
// We return undefined, instead of nothing here, so it's
// easier to handle this case. if(!global) { ...}

module.exports = g;


/***/ }),
/* 1 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
/* WEBPACK VAR INJECTION */(function(global) {

Object.defineProperty(exports, "__esModule", {
  value: true
});

var _dotnetify2 = __webpack_require__(15);

var _dotnetify3 = _interopRequireDefault(_dotnetify2);

var _dotnetifyVm = __webpack_require__(5);

var _dotnetifyVm2 = _interopRequireDefault(_dotnetifyVm);

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

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
var window = window || global || {};
var dotnetify = window.dotnetify || _dotnetify3.default;

dotnetify.react = {
  version: '3.0.0',
  viewModels: {},
  plugins: {},
  controller: dotnetify,

  // Internal variables.
  _hubs: [],

  // Initializes connection to SignalR server hub.
  init: function init(iHub) {
    var self = dotnetify.react;
    var hubInitialized = self._hubs.some(function (hub) {
      return hub === iHub;
    });

    var start = function start() {
      if (!iHub.isHubStarted) Object.keys(self.viewModels).filter(function (vmId) {
        return self.viewModels[vmId].$hub === iHub;
      }).forEach(function (vmId) {
        return self.viewModels[vmId].$requested = false;
      });

      dotnetify.startHub(iHub);
    };

    if (!hubInitialized) {
      iHub.responseEvent.subscribe(function (iVMId, iVMData) {
        return self._responseVM(iVMId, iVMData);
      });
      iHub.connectedEvent.subscribe(function () {
        return Object.keys(self.viewModels).filter(function (vmId) {
          return self.viewModels[vmId].$hub === iHub && !self.viewModels[vmId].$requested;
        }).forEach(function (vmId) {
          return self.viewModels[vmId].$request();
        });
      });
      iHub.reconnectedEvent.subscribe(start);

      if (iHub.mode !== 'local') self._hubs.push(iHub);
    }

    start();
  },

  // Connects to a server view model.
  connect: function connect(iVMId, iReact, iOptions) {
    if (arguments.length < 2) throw new Error('[dotNetify] Missing arguments. Usage: connect(vmId, component) ');

    var vmArg = iOptions && iOptions['vmArg'];

    if (dotnetify.ssr && dotnetify.react.ssrConnect) {
      return dotnetify.react.ssrConnect(iVMId, iReact, vmArg);
    }

    var self = dotnetify.react;
    if (self.viewModels.hasOwnProperty(iVMId)) {
      console.error('Component is attempting to connect to an already active \'' + iVMId + '\'. ' + ' If it\'s from a dismounted component, you must add vm.$destroy to componentWillUnmount().');
      self.viewModels[iVMId].$destroy();
      return setTimeout(function () {
        return self.connect(iVMId, iReact, iOptions);
      });
    }

    var component = {
      get props() {
        return iReact.props;
      },
      get state() {
        return iReact.state;
      },
      setState: function setState(state) {
        iReact.setState(state);
      }
    };

    var connectInfo = dotnetify.selectHub({ vmId: iVMId, options: iOptions, hub: null });
    self.viewModels[iVMId] = new _dotnetifyVm2.default(connectInfo.vmId, component, connectInfo.options, self, connectInfo.hub);
    if (connectInfo.hub) self.init(connectInfo.hub);

    return self.viewModels[iVMId];
  },

  // Get all view models.
  getViewModels: function getViewModels() {
    var self = dotnetify.react;
    return Object.keys(self.viewModels).map(function (vmId) {
      return self.viewModels[vmId];
    });
  },

  _responseVM: function _responseVM(iVMId, iVMData) {
    var self = dotnetify.react;

    if (self.viewModels.hasOwnProperty(iVMId)) {
      var vm = self.viewModels[iVMId];
      dotnetify.checkServerSideException(iVMId, iVMData, vm.$exceptionHandler);
      vm.$update(iVMData);
      return true;
    }
    return false;
  }
};

dotnetify.addVMAccessor(dotnetify.react.getViewModels);

exports.default = dotnetify;
/* WEBPACK VAR INJECTION */}.call(this, __webpack_require__(0)))

/***/ }),
/* 2 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
/* WEBPACK VAR INJECTION */(function(global) {

Object.defineProperty(exports, "__esModule", {
  value: true
});

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _toConsumableArray(arr) { if (Array.isArray(arr)) { for (var i = 0, arr2 = Array(arr.length); i < arr.length; i++) { arr2[i] = arr[i]; } return arr2; } else { return Array.from(arr); } }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

/* 
Copyright 2018 Dicky Suryadi

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
var window = window || global || {};

var utils = function () {
  function utils() {
    _classCallCheck(this, utils);
  }

  _createClass(utils, [{
    key: 'trim',

    // Trim slashes from start and end of string.
    value: function trim(iStr) {
      if (typeof iStr !== 'string') return '';

      while (iStr.indexOf('/', iStr.length - 1) >= 0) {
        iStr = iStr.substr(0, iStr.length - 1);
      }while (iStr.indexOf('/') == 0) {
        iStr = iStr.substr(1, iStr.length - 1);
      }return iStr;
    }

    // Match two strings case-insensitive.

  }, {
    key: 'equal',
    value: function equal(iStr1, iStr2) {
      return iStr1 != null && iStr2 != null && iStr1.toLowerCase() == iStr2.toLowerCase();
    }

    // Whether the string starts or ends with a value.

  }, {
    key: 'startsWith',
    value: function startsWith(iStr, iValue) {
      return iStr.toLowerCase().slice(0, iValue.length) == iValue.toLowerCase();
    }
  }, {
    key: 'endsWith',
    value: function endsWith(iStr, iValue) {
      return iValue == '' || iStr.toLowerCase().slice(-iValue.length) == iValue.toLowerCase();
    }

    // Dispatch event with IE polyfill.

  }, {
    key: 'dispatchEvent',
    value: function dispatchEvent(iEvent) {
      if (typeof Event === 'function') window.dispatchEvent(new Event(iEvent));else {
        var event = document.createEvent('CustomEvent');
        event.initEvent(iEvent, true, true);
        window.dispatchEvent(event);
      }
    }
  }, {
    key: 'grep',
    value: function grep(iArray, iFilter) {
      return Array.isArray(iArray) ? iArray.filter(iFilter) : [];
    }
  }]);

  return utils;
}();

var createEventEmitter = exports.createEventEmitter = function createEventEmitter(_) {
  var subscribers = [];
  return {
    emit: function emit() {
      for (var _len = arguments.length, args = Array(_len), _key = 0; _key < _len; _key++) {
        args[_key] = arguments[_key];
      }

      var handled = false;
      subscribers.forEach(function (subscriber) {
        handled = subscriber.apply(undefined, _toConsumableArray(args)) || handled;
      });
      return handled;
    },
    subscribe: function subscribe(subscriber) {
      !subscribers.includes(subscriber) && subscribers.push(subscriber);
      return function () {
        return subscribers = subscribers.filter(function (x) {
          return x !== subscriber;
        });
      };
    }
  };
};

var fetch = exports.fetch = function fetch(iMethod, iUrl, iData, iOptions) {
  return new Promise(function (resolve, reject) {
    var request = new window.XMLHttpRequest();
    request.open(iMethod, iUrl, true);
    if (typeof iOptions == 'function') iOptions(request);

    request.onload = function () {
      if (request.status >= 200 && request.status < 400) {
        var response = request.responseText;
        resolve(response);
      } else reject(request);
    };
    request.onerror = function () {
      reject(request);
    };
    request.send(iData);
  });
};

exports.default = new utils();
/* WEBPACK VAR INJECTION */}.call(this, __webpack_require__(0)))

/***/ }),
/* 3 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
/* WEBPACK VAR INJECTION */(function(global, module) {

var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) { return typeof obj; } : function (obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; };

/* 
Copyright 2017 Dicky Suryadi

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

var window = window || global || {};

var jQueryDeferred = __webpack_require__(12);
var jQueryShim = jQueryDeferred.extend(function (selector) {
  if (selector === window || selector.document) return {
    0: selector,
    on: function on(iEvent, iHandler) {
      window.addEventListener(iEvent, iHandler);
    },
    bind: function bind(iEvent, iHandler) {
      window.addEventListener(iEvent, iHandler, false);
    },
    unbind: function unbind(iEvent, iHandler) {
      window.removeEventListener(iEvent, iHandler, false);
    }
  };

  if (typeof selector !== 'string') selector.events = selector.events || {};

  return {
    0: selector,

    bind: function bind(iEvent, iHandler) {
      var event = selector.events[iEvent] || [];
      event.push(iHandler);
      selector.events[iEvent] = event;
    },

    unbind: function unbind(iEvent, iHandler) {
      var handlers = selector.events[iEvent] || [];
      if (iHandler) {
        var idx = handlers.indexOf(iHandler);
        if (idx !== -1) handlers.splice(idx, 1);
      } else handlers = [];
      selector.events[iEvent] = handlers;
    },

    triggerHandler: function triggerHandler(iEvent, iArgs) {
      var handlers = selector.events[iEvent] || [];
      var args = [{ type: iEvent }];
      if (Array.isArray(iArgs)) iArgs.forEach(function (arg) {
        args.push(arg);
      });else if (iArgs) args.push(iArgs);
      handlers.forEach(function (handler) {
        handler.apply(this, args);
      });
    },

    load: function load(iUrl, iArgs, iHandler) {
      var request = new window.XMLHttpRequest();
      request.open('GET', iUrl, true);
      request.onload = function () {
        if (request.status >= 200 && request.status < 400) {
          var response = request.responseText;
          document.querySelector(selector).innerHTML = response;
          iHandler.call(document.querySelector(selector));
        }
      };
      request.send();
      return {
        abort: function abort(reason) {
          return request.abort(reason);
        }
      };
    }
  };
}, jQueryDeferred, {
  support: { cors: true },

  trim: function trim(iStr) {
    return typeof iStr === 'string' ? iStr.trim() : iStr;
  },

  inArray: function inArray(iArray, iItem) {
    return iArray.indexOf(iItem) !== -1;
  },

  makeArray: function makeArray(iArray) {
    return [].slice.call(iArray, 0);
  },

  merge: function merge(iArray1, iArray2) {
    Array.prototype.push.apply(iArray1, iArray2);
    return iArray1;
  },

  isEmptyObject: function isEmptyObject(iObj) {
    return !iObj || Object.keys(iObj).length === 0;
  },

  ajax: function ajax(iOptions) {
    var request = new window.XMLHttpRequest();
    request.onreadystatechange = function () {
      if (request.readyState !== 4) return;
      if (request.status === 200 && !request._hasError) {
        try {
          iOptions.success && iOptions.success(JSON.parse(request.responseText));
        } catch (error) {
          iOptions.success && iOptions.success(request.responseText);
        }
      } else iOptions.error && iOptions.error(request);
    };
    request.open(iOptions.type, iOptions.url);
    request.setRequestHeader('content-type', iOptions.contentType);
    request.send(iOptions.data.data && 'data=' + iOptions.data.data);
    return {
      abort: function abort(reason) {
        return request.abort(reason);
      }
    };
  },

  getScript: function getScript(iUrl, iSuccess) {
    var done = false;
    var promise = jQueryDeferred.Deferred();
    var head = document.getElementsByTagName('head')[0];
    var script = document.createElement('script');
    script.src = iUrl;
    script.onload = script.onreadystatechange = function () {
      if (!done && (!this.readyState || this.readyState == 'loaded' || this.readyState == 'complete')) {
        done = true;
        script.onload = script.onreadystatechange = null;
        head.removeChild(script);
        if (typeof iSuccess === 'function') iSuccess();
        promise.resolve();
      }
    };
    head.appendChild(script);
    return promise;
  }
});

if (typeof window !== 'undefined') window.jQuery = window.jQuery || jQueryShim;

if (( false ? undefined : _typeof(exports)) === 'object' && ( false ? undefined : _typeof(module)) === 'object') module.exports = jQueryShim;
/* WEBPACK VAR INJECTION */}.call(this, __webpack_require__(0), __webpack_require__(13)(module)))

/***/ }),
/* 4 */,
/* 5 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
/* WEBPACK VAR INJECTION */(function(global) {

Object.defineProperty(exports, "__esModule", {
  value: true
});

var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) { return typeof obj; } : function (obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; };

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }(); /* 
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


var _jqueryShim = __webpack_require__(3);

var _jqueryShim2 = _interopRequireDefault(_jqueryShim);

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

var window = window || global || {};

// Client-side view model that acts as a proxy of the server view model.

var dotnetifyVM = function () {
  // iVMId - identifies the view model.
  // iComponent - component object.
  // iOptions - Optional configuration options:
  //    getState: state accessor.
  //    setState: state mutator.
  //    vmArg: view model arguments.
  //    headers: request headers, for things like authentication token.
  //    exceptionHandler: called when receiving server-side exception.
  // iDotNetify - framework-specific dotNetify library.
  // iHub - hub connection.
  function dotnetifyVM(iVMId, iComponent, iOptions, iDotNetify, iHub) {
    var _this = this;

    _classCallCheck(this, dotnetifyVM);

    this.$vmId = iVMId;
    this.$component = iComponent;
    this.$options = iOptions || {};
    this.$vmArg = iOptions && iOptions['vmArg'];
    this.$headers = iOptions && iOptions['headers'];
    this.$exceptionHandler = iOptions && iOptions['exceptionHandler'];
    this.$requested = false;
    this.$loaded = false;
    this.$itemKey = {};
    this.$dotnetify = iDotNetify;
    this.$hub = iHub;

    var getState = iOptions && iOptions['getState'];
    var setState = iOptions && iOptions['setState'];
    getState = typeof getState === 'function' ? getState : function () {
      return iComponent.state;
    };
    setState = typeof setState === 'function' ? setState : function (state) {
      return iComponent.setState(state);
    };

    this.State = function (state) {
      return typeof state === 'undefined' ? getState() : setState(state);
    };
    this.Props = function (prop) {
      return _this.$component.props && _this.$component.props[prop];
    };

    var vmArg = this.Props('vmArg');
    if (vmArg) this.$vmArg = _jqueryShim2.default.extend(this.$vmArg, vmArg);

    // Inject plugin functions into this view model.
    this.$getPlugins().map(function (plugin) {
      return typeof plugin['$inject'] == 'function' ? plugin.$inject(_this) : null;
    });
  }

  // Disposes the view model, both here and on the server.


  _createClass(dotnetifyVM, [{
    key: '$destroy',
    value: function $destroy() {
      var _this2 = this;

      // Call any plugin's $destroy function if provided.
      this.$getPlugins().map(function (plugin) {
        return typeof plugin['$destroy'] == 'function' ? plugin.$destroy.apply(_this2) : null;
      });

      if (this.$hub.isConnected) {
        try {
          this.$hub.disposeVM(this.$vmId);
        } catch (ex) {
          this.$dotnetify.controller.handleConnectionStateChanged('error', ex, this.$hub);
        }
      }

      delete this.$dotnetify.viewModels[this.$vmId];
    }

    // Dispatches a value to the server view model.
    // iValue - Value to dispatch.

  }, {
    key: '$dispatch',
    value: function $dispatch(iValue) {
      if (this.$hub.isConnected) {
        var controller = this.$dotnetify.controller;
        try {
          this.$hub.updateVM(this.$vmId, iValue);

          if (controller.debug) {
            console.log('[' + this.$vmId + '] sent> ');
            console.log(iValue);

            controller.debugFn && controller.debugFn(this.$vmId, 'sent', iValue);
          }
        } catch (ex) {
          controller.handleConnectionStateChanged('error', ex, this.$hub);
        }
      }
    }

    // Dispatches a state value to the server view model.
    // iValue - State value to dispatch.

  }, {
    key: '$dispatchListState',
    value: function $dispatchListState(iValue) {
      var _this3 = this;

      var _loop = function _loop() {
        var key = _this3.$itemKey[listName];
        if (!key) {
          console.error('[' + _this3.$vmId + '] missing item key for \'' + listName + '\'; add ' + listName + '_itemKey property to the view model.');
          return {
            v: void 0
          };
        }
        item = iValue[listName];

        if (!item[key]) {
          console.error('[' + _this3.$vmId + '] couldn\'t dispatch data from \'' + listName + '\' due to missing property \'' + key + '\'.');
          console.error(item);
          return {
            v: void 0
          };
        }

        Object.keys(item).forEach(function (prop) {
          if (prop != key) {
            var state = {};
            state[listName + '.$' + item[key] + '.' + prop] = item[prop];
            _this3.$dispatch(state);
          }
        });
        _this3.$updateList(listName, item);
      };

      for (var listName in iValue) {
        var item;

        var _ret = _loop();

        if ((typeof _ret === 'undefined' ? 'undefined' : _typeof(_ret)) === "object") return _ret.v;
      }
    }
  }, {
    key: '$getPlugins',
    value: function $getPlugins() {
      var _this4 = this;

      return Object.keys(this.$dotnetify.plugins).map(function (id) {
        return _this4.$dotnetify.plugins[id];
      });
    }

    // Preprocess view model update from the server before we set the state.

  }, {
    key: '$preProcess',
    value: function $preProcess(iVMUpdate) {
      var vm = this;

      for (var prop in iVMUpdate) {
        // Look for property that end with '_add'. Interpret the value as a list item to be added
        // to an existing list whose property name precedes that suffix.
        var match = /(.*)_add/.exec(prop);
        if (match != null) {
          var listName = match[1];
          if (Array.isArray(this.State()[listName])) vm.$addList(listName, iVMUpdate[prop]);else console.error('unable to resolve ' + prop);
          delete iVMUpdate[prop];
          continue;
        }

        // Look for property that end with '_update'. Interpret the value as a list item to be updated
        // to an existing list whose property name precedes that suffix.
        var match = /(.*)_update/.exec(prop);
        if (match != null) {
          var listName = match[1];
          if (Array.isArray(this.State()[listName])) vm.$updateList(listName, iVMUpdate[prop]);else console.error('[' + this.$vmId + "] '" + listName + "' is not found or not an array.");
          delete iVMUpdate[prop];
          continue;
        }

        // Look for property that end with '_remove'. Interpret the value as a list item key to remove
        // from an existing list whose property name precedes that suffix.
        var match = /(.*)_remove/.exec(prop);
        if (match != null) {
          var listName = match[1];
          if (Array.isArray(this.State()[listName])) {
            var key = this.$itemKey[listName];
            if (key != null) {
              if (Array.isArray(iVMUpdate[prop])) vm.$removeList(listName, function (i) {
                return iVMUpdate[prop].some(function (x) {
                  return i[key] == x;
                });
              });else vm.$removeList(listName, function (i) {
                return i[key] == iVMUpdate[prop];
              });
            } else console.error('[' + this.$vmId + '] missing item key for \'' + listName + '\'; add ' + listName + '_itemKey property to the view model.');
          } else console.error('[' + this.$vmId + '] \'' + listName + '\' is not found or not an array.');
          delete iVMUpdate[prop];
          continue;
        }

        // Look for property that end with '_itemKey'. Interpret the value as the property name that will
        // uniquely identify items in the list.
        var match = /(.*)_itemKey/.exec(prop);
        if (match != null) {
          var listName = match[1];
          var itemKey = {};
          itemKey[listName] = iVMUpdate[prop];
          vm.$setItemKey(itemKey);
          delete iVMUpdate[prop];
          continue;
        }
      }
    }

    // Requests state from the server view model.

  }, {
    key: '$request',
    value: function $request() {
      if (this.$hub.isConnected) this.$hub.requestVM(this.$vmId, { $vmArg: this.$vmArg, $headers: this.$headers });
      this.$requested = true;
    }

    // Updates state from the server view model to the view.
    // iVMData - Serialized state from the server.

  }, {
    key: '$update',
    value: function $update(iVMData) {
      var controller = this.$dotnetify.controller;
      if (controller.debug) {
        console.log('[' + this.$vmId + '] received> ');
        console.log(JSON.parse(iVMData));

        controller.debugFn && controller.debugFn(this.$vmId, 'received', JSON.parse(iVMData));
      }
      var vmData = JSON.parse(iVMData);
      this.$preProcess(vmData);

      var state = this.State();
      state = _jqueryShim2.default.extend({}, state, vmData);
      this.State(state);

      if (!this.$loaded) this.$onLoad();else this.$onUpdate(vmData);
    }

    // Handles initial view model load event.

  }, {
    key: '$onLoad',
    value: function $onLoad() {
      var _this5 = this;

      this.$getPlugins().map(function (plugin) {
        return typeof plugin['$ready'] == 'function' ? plugin.$ready.apply(_this5) : null;
      });
      this.$loaded = true;
    }

    // Handles view model update event.

  }, {
    key: '$onUpdate',
    value: function $onUpdate(vmData) {
      var _this6 = this;

      this.$getPlugins().map(function (plugin) {
        return typeof plugin['$update'] == 'function' ? plugin.$update.apply(_this6, [vmData]) : null;
      });
    }

    // *** CRUD Functions ***

    // Sets items key to identify individual items in a list.
    // Accepts object literal: { "<list name>": "<key prop name>", ... }

  }, {
    key: '$setItemKey',
    value: function $setItemKey(iItemKey) {
      Object.assign(this.$itemKey, iItemKey);
    }

    //// Adds a new item to a state array.

  }, {
    key: '$addList',
    value: function $addList(iListName, iNewItem) {
      var _this7 = this;

      var items = this.State()[iListName];

      if (Array.isArray(iNewItem) && !Array.isArray(items[0] || [])) {
        iNewItem.forEach(function (item) {
          return _this7.$addList(iListName, item);
        });
        return;
      }

      // Check if the list already has an item with the same key. If so, replace it.
      var key = this.$itemKey[iListName];
      if (key != null) {
        if (!iNewItem.hasOwnProperty(key)) {
          console.error('[' + this.$vmId + '] couldn\'t add item to \'' + iListName + '\' due to missing property \'' + key + '\'.');
          return;
        }
        var match = this.State()[iListName].filter(function (i) {
          return i[key] == iNewItem[key];
        });
        if (match.length > 0) {
          console.error('[' + this.$vmId + '] couldn\'t add item to \'' + iListName + '\' because the key already exists.');
          return;
        }
      }
      items.push(iNewItem);

      var state = {};
      state[iListName] = items;
      this.State(state);
    }

    // Removes an item from a state array.

  }, {
    key: '$removeList',
    value: function $removeList(iListName, iFilter) {
      var state = {};
      state[iListName] = this.State()[iListName].filter(function (i) {
        return !iFilter(i);
      });
      this.State(state);
    }

    //// Updates existing item to an observable array.

  }, {
    key: '$updateList',
    value: function $updateList(iListName, iNewItem) {
      var _this8 = this;

      var items = this.State()[iListName];

      if (Array.isArray(iNewItem) && !Array.isArray(items[0] || [])) {
        iNewItem.forEach(function (item) {
          return _this8.$updateList(iListName, item);
        });
        return;
      }

      // Check if the list already has an item with the same key. If so, update it.
      var key = this.$itemKey[iListName];
      if (key != null) {
        if (!iNewItem.hasOwnProperty(key)) {
          console.error('[' + this.$vmId + '] couldn\'t update item to \'' + iListName + '\' due to missing property \'' + key + '\'.');
          return;
        }
        var state = {};
        state[iListName] = items.map(function (i) {
          return i[key] == iNewItem[key] ? _jqueryShim2.default.extend(i, iNewItem) : i;
        });
        this.State(state);
      } else console.error('[' + this.$vmId + '] missing item key for \'' + iListName + '\'; add \'' + iListName + '_itemKey\' property to the view model.');
    }
  }]);

  return dotnetifyVM;
}();

exports.default = dotnetifyVM;
/* WEBPACK VAR INJECTION */}.call(this, __webpack_require__(0)))

/***/ }),
/* 6 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.createWebApiHub = exports.dotNetifyHubWebApi = undefined;

var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) { return typeof obj; } : function (obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; };

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }(); /* 
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


var _utils = __webpack_require__(2);

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

var dotNetifyHubWebApi = exports.dotNetifyHubWebApi = function () {
  function dotNetifyHubWebApi(iBaseUrl, iOnRequest) {
    _classCallCheck(this, dotNetifyHubWebApi);

    this.mode = 'webapi';
    this.debug = false;
    this.isConnected = false;
    this.isHubStarted = false;
    this.responseEvent = (0, _utils.createEventEmitter)();
    this.reconnectedEvent = (0, _utils.createEventEmitter)();
    this.connectedEvent = (0, _utils.createEventEmitter)();
    this.connectionFailedEvent = (0, _utils.createEventEmitter)();
    this._vmArgs = {};

    this.baseUrl = iBaseUrl || '';
    this.onRequest = iOnRequest;
  }

  _createClass(dotNetifyHubWebApi, [{
    key: 'startHub',
    value: function startHub() {
      this.isConnected = true;
      this.isHubStarted = true;
      this.connectedEvent.emit();
    }
  }, {
    key: 'requestVM',
    value: function requestVM(iVMId, iVMArgs) {
      var _this = this;

      var vmArgs = iVMArgs || {};
      var vmArgQuery = vmArgs.$vmArg ? '?vmarg=' + JSON.stringify(vmArgs.$vmArg) : '';
      var headers = vmArgs.$headers || {};

      this._vmArgs[iVMId] = vmArgs;
      var url = this.baseUrl + ('/api/dotnetify/vm/' + iVMId + vmArgQuery);

      (0, _utils.fetch)('GET', url, null, function (request) {
        Object.keys(headers).forEach(function (key) {
          return request.setRequestHeader(key, headers[key]);
        });
        if (typeof _this.onRequest == 'function') _this.onRequest(url, request);
      }).then(function (response) {
        _this.responseEvent.emit(iVMId, response);
      }).catch(function (request) {
        return console.error('[' + iVMId + '] Request failed', request);
      });
    }
  }, {
    key: 'updateVM',
    value: function updateVM(iVMId, iValue) {
      var _this2 = this;

      var vmArgs = this._vmArgs[iVMId] || {};
      var vmArgQuery = vmArgs.$vmArg ? '?vmarg=' + JSON.stringify(vmArgs.$vmArg) : '';
      var headers = vmArgs.$headers || {};
      var payload = (typeof iValue === 'undefined' ? 'undefined' : _typeof(iValue)) == 'object' ? JSON.stringify(iValue) : iValue;

      var url = this.baseUrl + ('/api/dotnetify/vm/' + iVMId + vmArgQuery);

      (0, _utils.fetch)('POST', url, payload, function (request) {
        request.setRequestHeader('Content-Type', 'application/json');
        Object.keys(headers).forEach(function (key) {
          return request.setRequestHeader(key, headers[key]);
        });
        if (typeof _this2.onRequest == 'function') _this2.onRequest(url, request, payload);
      }).then(function (response) {
        _this2.responseEvent.emit(iVMId, response);
      }).catch(function (request) {
        return console.error('[' + iVMId + '] Update failed', request);
      });
    }
  }, {
    key: 'disposeVM',
    value: function disposeVM(iVMId) {
      delete this._vmArgs[iVMId];
    }
  }], [{
    key: 'create',
    value: function create(iBaseUrl, iOnRequest) {
      return new dotNetifyHubWebApi(iBaseUrl, iOnRequest);
    }
  }]);

  return dotNetifyHubWebApi;
}();

var createWebApiHub = dotNetifyHubWebApi.create;

exports.createWebApiHub = createWebApiHub;
exports.default = dotNetifyHubWebApi.create();

/***/ }),
/* 7 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
/* WEBPACK VAR INJECTION */(function(global) {

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.hasLocalVM = exports.dotNetifyHubLocal = undefined;

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) { return typeof obj; } : function (obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; /* 
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


var _utils = __webpack_require__(2);

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

var window = window || global || {};

var normalize = function normalize(iVMId) {
  return iVMId && iVMId.replace(/\./g, '_');
};
var hasLocalVM = function hasLocalVM(iVMId) {
  var vmId = normalize(iVMId);
  var vm = window[vmId];
  return (typeof vm === 'undefined' ? 'undefined' : _typeof(vm)) == 'object' && typeof vm.onConnect == 'function';
};

var dotNetifyHubLocal = exports.dotNetifyHubLocal = function () {
  function dotNetifyHubLocal() {
    _classCallCheck(this, dotNetifyHubLocal);

    this.mode = 'local';
    this.debug = false;
    this.isConnected = false;
    this.isHubStarted = false;
    this.responseEvent = (0, _utils.createEventEmitter)();
    this.reconnectedEvent = (0, _utils.createEventEmitter)();
    this.connectedEvent = (0, _utils.createEventEmitter)();
    this.connectionFailedEvent = (0, _utils.createEventEmitter)();
  }

  _createClass(dotNetifyHubLocal, [{
    key: 'startHub',
    value: function startHub() {
      this.isConnected = true;
      this.isHubStarted = true;
      this.connectedEvent.emit();
    }
  }, {
    key: 'requestVM',
    value: function requestVM(iVMId, iVMArgs) {
      var _this = this;

      var vmId = normalize(iVMId);
      var vm = window[vmId];

      if ((typeof vm === 'undefined' ? 'undefined' : _typeof(vm)) === 'object' && typeof vm.onConnect == 'function') {
        if (this.debug) console.log('[' + iVMId + '] *** local mode ***');

        vm.$pushUpdate = function (update) {
          if ((typeof update === 'undefined' ? 'undefined' : _typeof(update)) == 'object') update = JSON.stringify(update);
          setTimeout(function () {
            return _this.responseEvent.emit(iVMId, update);
          });
        };

        vm.$pushUpdate(vm.onConnect(iVMArgs) || {});
      }
    }
  }, {
    key: 'updateVM',
    value: function updateVM(iVMId, iValue) {
      var _this2 = this;

      var vmId = normalize(iVMId);
      var vm = window[vmId];

      if ((typeof vm === 'undefined' ? 'undefined' : _typeof(vm)) === 'object' && typeof vm.onDispatch == 'function') {
        var state = vm.onDispatch(iValue);
        if (state) {
          if ((typeof state === 'undefined' ? 'undefined' : _typeof(state)) == 'object') state = JSON.stringify(state);
          setTimeout(function () {
            return _this2.responseEvent.emit(iVMId, state);
          });
        }
      }
    }
  }, {
    key: 'disposeVM',
    value: function disposeVM(iVMId) {
      var vmId = normalize(iVMId);
      var vm = window[vmId];

      if ((typeof vm === 'undefined' ? 'undefined' : _typeof(vm)) === 'object' && typeof vm.onDestroy == 'function') {
        vm.onDestroy(iVMId);
      }
    }
  }]);

  return dotNetifyHubLocal;
}();

exports.default = new dotNetifyHubLocal();
exports.hasLocalVM = hasLocalVM;
/* WEBPACK VAR INJECTION */}.call(this, __webpack_require__(0)))

/***/ }),
/* 8 */
/***/ (function(module, exports) {

module.exports = __WEBPACK_EXTERNAL_MODULE__8__;

/***/ }),
/* 9 */
/***/ (function(module, exports) {

/**
* jQuery core object.
*
* Worker with jQuery deferred
*
* Code from: https://github.com/jquery/jquery/blob/master/src/core.js
*
*/

var jQuery = module.exports = {
	type: type
	, isArray: isArray
	, isFunction: isFunction
	, isPlainObject: isPlainObject
	, each: each
	, extend: extend
	, noop: function() {}
};

var toString = Object.prototype.toString;

var class2type = {};
// Populate the class2type map
"Boolean Number String Function Array Date RegExp Object".split(" ").forEach(function(name) {
	class2type[ "[object " + name + "]" ] = name.toLowerCase();
});


function type( obj ) {
	return obj == null ?
		String( obj ) :
			class2type[ toString.call(obj) ] || "object";
}

function isFunction( obj ) {
	return jQuery.type(obj) === "function";
}

function isArray( obj ) {
	return jQuery.type(obj) === "array";
}

function each( object, callback, args ) {
	var name, i = 0,
	length = object.length,
	isObj = length === undefined || isFunction( object );

	if ( args ) {
		if ( isObj ) {
			for ( name in object ) {
				if ( callback.apply( object[ name ], args ) === false ) {
					break;
				}
			}
		} else {
			for ( ; i < length; ) {
				if ( callback.apply( object[ i++ ], args ) === false ) {
					break;
				}
			}
		}

		// A special, fast, case for the most common use of each
	} else {
		if ( isObj ) {
			for ( name in object ) {
				if ( callback.call( object[ name ], name, object[ name ] ) === false ) {
					break;
				}
			}
		} else {
			for ( ; i < length; ) {
				if ( callback.call( object[ i ], i, object[ i++ ] ) === false ) {
					break;
				}
			}
		}
	}

	return object;
}

function isPlainObject( obj ) {
	// Must be an Object.
	if ( !obj || jQuery.type(obj) !== "object" ) {
		return false;
	}
	return true;
}

function extend() {
	var options, name, src, copy, copyIsArray, clone,
	target = arguments[0] || {},
	i = 1,
	length = arguments.length,
	deep = false;

	// Handle a deep copy situation
	if ( typeof target === "boolean" ) {
		deep = target;
		target = arguments[1] || {};
		// skip the boolean and the target
		i = 2;
	}

	// Handle case when target is a string or something (possible in deep copy)
	if ( typeof target !== "object" && !jQuery.isFunction(target) ) {
		target = {};
	}

	// extend jQuery itself if only one argument is passed
	if ( length === i ) {
		target = this;
		--i;
	}

	for ( ; i < length; i++ ) {
		// Only deal with non-null/undefined values
		if ( (options = arguments[ i ]) != null ) {
			// Extend the base object
			for ( name in options ) {
				src = target[ name ];
				copy = options[ name ];

				// Prevent never-ending loop
				if ( target === copy ) {
					continue;
				}

				// Recurse if we're merging plain objects or arrays
				if ( deep && copy && ( jQuery.isPlainObject(copy) || (copyIsArray = jQuery.isArray(copy)) ) ) {
					if ( copyIsArray ) {
						copyIsArray = false;
						clone = src && jQuery.isArray(src) ? src : [];

					} else {
						clone = src && jQuery.isPlainObject(src) ? src : {};
					}

					// Never move original objects, clone them
					target[ name ] = jQuery.extend( deep, clone, copy );

					// Don't bring in undefined values
				} else if ( copy !== undefined ) {
					target[ name ] = copy;
				}
			}
		}
	}

	// Return the modified object
	return target;
};




/***/ }),
/* 10 */
/***/ (function(module, exports, __webpack_require__) {

var jQuery = module.exports = __webpack_require__(9),
	core_rspace = /\s+/;
/**
* jQuery Callbacks
*
* Code from: https://github.com/jquery/jquery/blob/master/src/callbacks.js
*
*/


// String to Object options format cache
var optionsCache = {};

// Convert String-formatted options into Object-formatted ones and store in cache
function createOptions( options ) {
	var object = optionsCache[ options ] = {};
	jQuery.each( options.split( core_rspace ), function( _, flag ) {
		object[ flag ] = true;
	});
	return object;
}

/*
 * Create a callback list using the following parameters:
 *
 *	options: an optional list of space-separated options that will change how
 *			the callback list behaves or a more traditional option object
 *
 * By default a callback list will act like an event callback list and can be
 * "fired" multiple times.
 *
 * Possible options:
 *
 *	once:			will ensure the callback list can only be fired once (like a Deferred)
 *
 *	memory:			will keep track of previous values and will call any callback added
 *					after the list has been fired right away with the latest "memorized"
 *					values (like a Deferred)
 *
 *	unique:			will ensure a callback can only be added once (no duplicate in the list)
 *
 *	stopOnFalse:	interrupt callings when a callback returns false
 *
 */
jQuery.Callbacks = function( options ) {

	// Convert options from String-formatted to Object-formatted if needed
	// (we check in cache first)
	options = typeof options === "string" ?
		( optionsCache[ options ] || createOptions( options ) ) :
		jQuery.extend( {}, options );

	var // Last fire value (for non-forgettable lists)
		memory,
		// Flag to know if list was already fired
		fired,
		// Flag to know if list is currently firing
		firing,
		// First callback to fire (used internally by add and fireWith)
		firingStart,
		// End of the loop when firing
		firingLength,
		// Index of currently firing callback (modified by remove if needed)
		firingIndex,
		// Actual callback list
		list = [],
		// Stack of fire calls for repeatable lists
		stack = !options.once && [],
		// Fire callbacks
		fire = function( data ) {
			memory = options.memory && data;
			fired = true;
			firingIndex = firingStart || 0;
			firingStart = 0;
			firingLength = list.length;
			firing = true;
			for ( ; list && firingIndex < firingLength; firingIndex++ ) {
				if ( list[ firingIndex ].apply( data[ 0 ], data[ 1 ] ) === false && options.stopOnFalse ) {
					memory = false; // To prevent further calls using add
					break;
				}
			}
			firing = false;
			if ( list ) {
				if ( stack ) {
					if ( stack.length ) {
						fire( stack.shift() );
					}
				} else if ( memory ) {
					list = [];
				} else {
					self.disable();
				}
			}
		},
		// Actual Callbacks object
		self = {
			// Add a callback or a collection of callbacks to the list
			add: function() {
				if ( list ) {
					// First, we save the current length
					var start = list.length;
					(function add( args ) {
						jQuery.each( args, function( _, arg ) {
							var type = jQuery.type( arg );
							if ( type === "function" ) {
								if ( !options.unique || !self.has( arg ) ) {
									list.push( arg );
								}
							} else if ( arg && arg.length && type !== "string" ) {
								// Inspect recursively
								add( arg );
							}
						});
					})( arguments );
					// Do we need to add the callbacks to the
					// current firing batch?
					if ( firing ) {
						firingLength = list.length;
					// With memory, if we're not firing then
					// we should call right away
					} else if ( memory ) {
						firingStart = start;
						fire( memory );
					}
				}
				return this;
			},
			// Remove a callback from the list
			remove: function() {
				if ( list ) {
					jQuery.each( arguments, function( _, arg ) {
						var index;
						while( ( index = jQuery.inArray( arg, list, index ) ) > -1 ) {
							list.splice( index, 1 );
							// Handle firing indexes
							if ( firing ) {
								if ( index <= firingLength ) {
									firingLength--;
								}
								if ( index <= firingIndex ) {
									firingIndex--;
								}
							}
						}
					});
				}
				return this;
			},
			// Control if a given callback is in the list
			has: function( fn ) {
				return jQuery.inArray( fn, list ) > -1;
			},
			// Remove all callbacks from the list
			empty: function() {
				list = [];
				return this;
			},
			// Have the list do nothing anymore
			disable: function() {
				list = stack = memory = undefined;
				return this;
			},
			// Is it disabled?
			disabled: function() {
				return !list;
			},
			// Lock the list in its current state
			lock: function() {
				stack = undefined;
				if ( !memory ) {
					self.disable();
				}
				return this;
			},
			// Is it locked?
			locked: function() {
				return !stack;
			},
			// Call all callbacks with the given context and arguments
			fireWith: function( context, args ) {
				args = args || [];
				args = [ context, args.slice ? args.slice() : args ];
				if ( list && ( !fired || stack ) ) {
					if ( firing ) {
						stack.push( args );
					} else {
						fire( args );
					}
				}
				return this;
			},
			// Call all the callbacks with the given arguments
			fire: function() {
				self.fireWith( this, arguments );
				return this;
			},
			// To know if the callbacks have already been called at least once
			fired: function() {
				return !!fired;
			}
		};

	return self;
};



/***/ }),
/* 11 */
/***/ (function(module, exports, __webpack_require__) {


/*!
* jquery-deferred
* Copyright(c) 2011 Hidden <zzdhidden@gmail.com>
* MIT Licensed
*/

/**
* Library version.
*/

var jQuery = module.exports = __webpack_require__(10),
	core_slice = Array.prototype.slice;

/**
* jQuery deferred
*
* Code from: https://github.com/jquery/jquery/blob/master/src/deferred.js
* Doc: http://api.jquery.com/category/deferred-object/
*
*/

jQuery.extend({

	Deferred: function( func ) {
		var tuples = [
				// action, add listener, listener list, final state
				[ "resolve", "done", jQuery.Callbacks("once memory"), "resolved" ],
				[ "reject", "fail", jQuery.Callbacks("once memory"), "rejected" ],
				[ "notify", "progress", jQuery.Callbacks("memory") ]
			],
			state = "pending",
			promise = {
				state: function() {
					return state;
				},
				always: function() {
					deferred.done( arguments ).fail( arguments );
					return this;
				},
				then: function( /* fnDone, fnFail, fnProgress */ ) {
					var fns = arguments;
					return jQuery.Deferred(function( newDefer ) {
						jQuery.each( tuples, function( i, tuple ) {
							var action = tuple[ 0 ],
								fn = fns[ i ];
							// deferred[ done | fail | progress ] for forwarding actions to newDefer
							deferred[ tuple[1] ]( jQuery.isFunction( fn ) ?
								function() {
									var returned = fn.apply( this, arguments );
									if ( returned && jQuery.isFunction( returned.promise ) ) {
										returned.promise()
											.done( newDefer.resolve )
											.fail( newDefer.reject )
											.progress( newDefer.notify );
									} else {
										newDefer[ action + "With" ]( this === deferred ? newDefer : this, [ returned ] );
									}
								} :
								newDefer[ action ]
							);
						});
						fns = null;
					}).promise();
				},
				// Get a promise for this deferred
				// If obj is provided, the promise aspect is added to the object
				promise: function( obj ) {
					return obj != null ? jQuery.extend( obj, promise ) : promise;
				}
			},
			deferred = {};

		// Keep pipe for back-compat
		promise.pipe = promise.then;

		// Add list-specific methods
		jQuery.each( tuples, function( i, tuple ) {
			var list = tuple[ 2 ],
				stateString = tuple[ 3 ];

			// promise[ done | fail | progress ] = list.add
			promise[ tuple[1] ] = list.add;

			// Handle state
			if ( stateString ) {
				list.add(function() {
					// state = [ resolved | rejected ]
					state = stateString;

				// [ reject_list | resolve_list ].disable; progress_list.lock
				}, tuples[ i ^ 1 ][ 2 ].disable, tuples[ 2 ][ 2 ].lock );
			}

			// deferred[ resolve | reject | notify ] = list.fire
			deferred[ tuple[0] ] = list.fire;
			deferred[ tuple[0] + "With" ] = list.fireWith;
		});

		// Make the deferred a promise
		promise.promise( deferred );

		// Call given func if any
		if ( func ) {
			func.call( deferred, deferred );
		}

		// All done!
		return deferred;
	},

	// Deferred helper
	when: function( subordinate /* , ..., subordinateN */ ) {
		var i = 0,
			resolveValues = core_slice.call( arguments ),
			length = resolveValues.length,

			// the count of uncompleted subordinates
			remaining = length !== 1 || ( subordinate && jQuery.isFunction( subordinate.promise ) ) ? length : 0,

			// the master Deferred. If resolveValues consist of only a single Deferred, just use that.
			deferred = remaining === 1 ? subordinate : jQuery.Deferred(),

			// Update function for both resolve and progress values
			updateFunc = function( i, contexts, values ) {
				return function( value ) {
					contexts[ i ] = this;
					values[ i ] = arguments.length > 1 ? core_slice.call( arguments ) : value;
					if( values === progressValues ) {
						deferred.notifyWith( contexts, values );
					} else if ( !( --remaining ) ) {
						deferred.resolveWith( contexts, values );
					}
				};
			},

			progressValues, progressContexts, resolveContexts;

		// add listeners to Deferred subordinates; treat others as resolved
		if ( length > 1 ) {
			progressValues = new Array( length );
			progressContexts = new Array( length );
			resolveContexts = new Array( length );
			for ( ; i < length; i++ ) {
				if ( resolveValues[ i ] && jQuery.isFunction( resolveValues[ i ].promise ) ) {
					resolveValues[ i ].promise()
						.done( updateFunc( i, resolveContexts, resolveValues ) )
						.fail( deferred.reject )
						.progress( updateFunc( i, progressContexts, progressValues ) );
				} else {
					--remaining;
				}
			}
		}

		// if we're not waiting on anything, resolve the master
		if ( !remaining ) {
			deferred.resolveWith( resolveContexts, resolveValues );
		}

		return deferred.promise();
	}
});


/***/ }),
/* 12 */
/***/ (function(module, exports, __webpack_require__) {


module.exports = __webpack_require__(11);

/***/ }),
/* 13 */
/***/ (function(module, exports) {

module.exports = function(module) {
	if (!module.webpackPolyfill) {
		module.deprecate = function() {};
		module.paths = [];
		// module.parent = undefined by default
		if (!module.children) module.children = [];
		Object.defineProperty(module, "loaded", {
			enumerable: true,
			get: function() {
				return module.l;
			}
		});
		Object.defineProperty(module, "id", {
			enumerable: true,
			get: function() {
				return module.i;
			}
		});
		module.webpackPolyfill = 1;
	}
	return module;
};


/***/ }),
/* 14 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
/* WEBPACK VAR INJECTION */(function(global) {

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.dotnetifyHubFactory = undefined;

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }(); /* 
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     Copyright 2017-2019 Dicky Suryadi
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
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


var _utils = __webpack_require__(2);

var _jqueryShim = __webpack_require__(3);

var _jqueryShim2 = _interopRequireDefault(_jqueryShim);

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

var signalRNetCore = __webpack_require__(8);
var $ = _jqueryShim2.default;

var window = window || global || {};

var dotnetifyHubFactory = exports.dotnetifyHubFactory = function () {
  function dotnetifyHubFactory() {
    _classCallCheck(this, dotnetifyHubFactory);
  }

  _createClass(dotnetifyHubFactory, null, [{
    key: 'create',
    value: function create() {
      var dotnetifyHub = {
        version: '2.0.0',
        type: null,

        reconnectDelay: [2, 5, 10],
        reconnectRetry: null,

        _startInfo: null,
        _init: false,

        // Hub server methods.
        requestVM: function requestVM(iVMId, iOptions) {
          return dotnetifyHub.server.request_VM(iVMId, iOptions);
        },
        updateVM: function updateVM(iVMId, iValue) {
          return dotnetifyHub.server.update_VM(iVMId, iValue);
        },
        disposeVM: function disposeVM(iVMId) {
          return dotnetifyHub.server.dispose_VM(iVMId);
        },

        // Connection events.
        responseEvent: (0, _utils.createEventEmitter)(),
        reconnectedEvent: (0, _utils.createEventEmitter)(),
        connectedEvent: (0, _utils.createEventEmitter)(),
        connectionFailedEvent: (0, _utils.createEventEmitter)(),

        get isHubStarted() {
          return !!this._startInfo;
        },

        // Starts connection with SignalR hub server.
        startHub: function startHub(hubOptions, doneHandler, failHandler, forceRestart) {
          var _this = this;

          var _doneHandler = function _doneHandler() {
            if (typeof doneHandler == 'function') doneHandler();
            _this.connectedEvent.emit();
          };
          var _failHandler = function _failHandler(ex) {
            if (typeof failHander == 'function') failHandler();
            _this.connectionFailedEvent.emit();
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
        signalR = signalR || window.signalR || signalRNetCore;

        // SignalR .NET Core.
        if (signalR && signalR.HubConnection) {
          dotnetifyHub.type = 'netcore';

          Object.defineProperty(dotnetifyHub, 'isConnected', {
            get: function get() {
              return dotnetifyHub._connection && dotnetifyHub._connection.connection.connectionState === 1;
            }
          });

          dotnetifyHub = $.extend(dotnetifyHub, {
            hubPath: iHubPath || '/dotnetify',
            url: iServerUrl,

            // Internal variables. Do not modify!
            _connection: null,
            _reconnectCount: 0,
            _startDoneHandler: null,
            _startFailHandler: null,
            _disconnectedHandler: function _disconnectedHandler() {},
            _stateChangedHandler: function _stateChangedHandler(iNewState) {},

            _onDisconnected: function _onDisconnected() {
              dotnetifyHub._changeState(4);
              dotnetifyHub._disconnectedHandler();
            },

            _changeState: function _changeState(iNewState) {
              if (iNewState == 1) dotnetifyHub._reconnectCount = 0;

              var stateText = { 0: 'connecting', 1: 'connected', 2: 'reconnecting', 4: 'disconnected', 99: 'terminated' };
              dotnetifyHub._stateChangedHandler(stateText[iNewState]);
            },

            _startConnection: function _startConnection(iHubOptions, iTransportArray) {
              var url = dotnetifyHub.url ? dotnetifyHub.url + dotnetifyHub.hubPath : dotnetifyHub.hubPath;
              var hubOptions = {};
              Object.keys(iHubOptions).forEach(function (key) {
                hubOptions[key] = iHubOptions[key];
              });
              hubOptions.transport = iTransportArray.shift();

              var hubConnectionBuilder = new signalR.HubConnectionBuilder().withUrl(url, hubOptions);
              if (typeof hubOptions.connectionBuilder == 'function') hubConnectionBuilder = hubOptions.connectionBuilder(hubConnectionBuilder);

              dotnetifyHub._connection = hubConnectionBuilder.build();
              dotnetifyHub._connection.on('response_vm', dotnetifyHub.client.response_VM);
              dotnetifyHub._connection.onclose(dotnetifyHub._onDisconnected);

              var promise = dotnetifyHub._connection.start().then(function () {
                dotnetifyHub._changeState(1);
              }).catch(function () {
                // If failed to start, fallback to the next transport.
                if (iTransportArray.length > 0) dotnetifyHub._startConnection(iHubOptions, iTransportArray);else dotnetifyHub._onDisconnected();
              });

              if (typeof dotnetifyHub._startDoneHandler === 'function') promise.then(dotnetifyHub._startDoneHandler).catch(dotnetifyHub._startFailHandler || function () {});
              return promise;
            },

            start: function start(iHubOptions) {
              dotnetifyHub._startDoneHandler = null;
              dotnetifyHub._startFailHandler = null;

              // Map the transport option.
              var transport = [0];
              var transportOptions = { webSockets: 0, serverSentEvents: 1, longPolling: 2 };
              if (iHubOptions && Array.isArray(iHubOptions.transport)) transport = iHubOptions.transport.map(function (arg) {
                return transportOptions[arg];
              });

              var promise = dotnetifyHub._startConnection(iHubOptions, transport);
              return {
                done: function done(iHandler) {
                  dotnetifyHub._startDoneHandler = iHandler;
                  promise.then(iHandler).catch(function (error) {
                    throw error;
                  });
                  return this;
                },
                fail: function fail(iHandler) {
                  dotnetifyHub._startFailHandler = iHandler;
                  promise.catch(iHandler);
                  return this;
                }
              };
            },

            disconnected: function disconnected(iHandler) {
              if (typeof iHandler === 'function') dotnetifyHub._disconnectedHandler = iHandler;
            },

            stateChanged: function stateChanged(iHandler) {
              if (typeof iHandler === 'function') dotnetifyHub._stateChangedHandler = iHandler;
            },

            reconnect: function reconnect(iStartHubFunc) {
              if (typeof iStartHubFunc === 'function') {
                // Only attempt reconnect if the specified retry hasn't been exceeded.
                if (!dotnetifyHub.reconnectRetry || dotnetifyHub._reconnectCount < dotnetifyHub.reconnectRetry) {
                  // Determine reconnect delay from the specified configuration array.
                  var delay = dotnetifyHub._reconnectCount < dotnetifyHub.reconnectDelay.length ? dotnetifyHub.reconnectDelay[dotnetifyHub._reconnectCount] : dotnetifyHub.reconnectDelay[dotnetifyHub.reconnectDelay.length - 1];

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
              dispose_VM: function dispose_VM(iVMId) {
                dotnetifyHub._connection.invoke('Dispose_VM', iVMId);
              },
              update_VM: function update_VM(iVMId, iValue) {
                dotnetifyHub._connection.invoke('Update_VM', iVMId, iValue);
              },
              request_VM: function request_VM(iVMId, iArgs) {
                dotnetifyHub._connection.invoke('Request_VM', iVMId, iArgs);
              }
            }
          });
        } else {
          // SignalR .NET FX.
          dotnetifyHub.type = 'netfx';

          if (window.jQuery) $ = window.jQuery;

          // SignalR hub auto-generated from /signalr/hubs.
          /// <reference path="..\..\SignalR.Client.JS\Scripts\jquery-1.6.4.js" />
          /// <reference path="jquery.signalR.js" />
          (function ($, window, undefined) {
            /// <param name="$" type="jQuery" />
            'use strict';

            if (typeof $.signalR !== 'function') {
              throw new Error('SignalR: SignalR is not loaded. Please ensure jquery.signalR-x.js is referenced before ~/signalr/js.');
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

              proxies['dotNetifyHub'] = this.createHubProxy('dotNetifyHub');
              proxies['dotNetifyHub'].client = {};
              proxies['dotNetifyHub'].server = {
                dispose_VM: function dispose_VM(vmId) {
                  return proxies['dotNetifyHub'].invoke.apply(proxies['dotNetifyHub'], $.merge(['Dispose_VM'], $.makeArray(arguments)));
                },

                request_VM: function request_VM(vmId, vmArg) {
                  return proxies['dotNetifyHub'].invoke.apply(proxies['dotNetifyHub'], $.merge(['Request_VM'], $.makeArray(arguments)));
                },

                update_VM: function update_VM(vmId, vmData) {
                  return proxies['dotNetifyHub'].invoke.apply(proxies['dotNetifyHub'], $.merge(['Update_VM'], $.makeArray(arguments)));
                }
              };

              return proxies;
            };

            signalR.hub = $.hubConnection(dotnetifyHub.hubPath, { useDefaultPath: false });
            $.extend(signalR, signalR.hub.createHubProxies());
          })($, window);

          Object.defineProperty(dotnetifyHub, 'state', {
            get: function get() {
              return $.connection.hub.state;
            },
            set: function set(val) {
              $.connection.hub.state = val;
            }
          });

          Object.defineProperty(dotnetifyHub, 'client', {
            get: function get() {
              return $.connection.dotNetifyHub.client;
            }
          });

          Object.defineProperty(dotnetifyHub, 'server', {
            get: function get() {
              return $.connection.dotNetifyHub.server;
            }
          });

          Object.defineProperty(dotnetifyHub, 'isConnected', {
            get: function get() {
              return $.connection.hub.state == $.signalR.connectionState.connected;
            }
          });

          dotnetifyHub = $.extend(dotnetifyHub, {
            hubPath: iHubPath || '/signalr',
            url: iServerUrl,

            _reconnectCount: 0,
            _stateChangedHandler: function _stateChangedHandler(iNewState) {},

            start: function start(iHubOptions) {
              if (dotnetifyHub.url) $.connection.hub.url = dotnetifyHub.url;

              var deferred = void 0;
              if (iHubOptions) deferred = $.connection.hub.start(iHubOptions);else deferred = $.connection.hub.start();
              deferred.fail(function (error) {
                if (error.source && error.source.message === 'Error parsing negotiate response.') console.warn('This client may be attempting to connect to an incompatible SignalR .NET Core server.');
              });
              return deferred;
            },

            disconnected: function disconnected(iHandler) {
              return $.connection.hub.disconnected(iHandler);
            },

            stateChanged: function stateChanged(iHandler) {
              dotnetifyHub._stateChangedHandler = iHandler;
              return $.connection.hub.stateChanged(function (state) {
                if (state == 1) dotnetifyHub._reconnectCount = 0;

                var stateText = { 0: 'connecting', 1: 'connected', 2: 'reconnecting', 4: 'disconnected' };
                iHandler(stateText[state.newState]);
              });
            },

            reconnect: function reconnect(iStartHubFunc) {
              if (typeof iStartHubFunc === 'function') {
                // Only attempt reconnect if the specified retry hasn't been exceeded.
                if (!dotnetifyHub.reconnectRetry || dotnetifyHub._reconnectCount < dotnetifyHub.reconnectRetry) {
                  // Determine reconnect delay from the specified configuration array.
                  var delay = dotnetifyHub._reconnectCount < dotnetifyHub.reconnectDelay.length ? dotnetifyHub.reconnectDelay[dotnetifyHub._reconnectCount] : dotnetifyHub.reconnectDelay[dotnetifyHub.reconnectDelay.length - 1];

                  dotnetifyHub._reconnectCount++;

                  setTimeout(function () {
                    dotnetifyHub._stateChangedHandler('reconnecting');
                    iStartHubFunc();
                  }, delay * 1000);
                } else dotnetifyHub._stateChangedHandler('terminated');
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

          var handled = dotnetifyHub.responseEvent.emit(iVMId, iVMData);

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
  }]);

  return dotnetifyHubFactory;
}();

exports.default = dotnetifyHubFactory.create();
/* WEBPACK VAR INJECTION */}.call(this, __webpack_require__(0)))

/***/ }),
/* 15 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.dotnetifyFactory = undefined;

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }(); /* 
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     Copyright 2017-2019 Dicky Suryadi
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
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


var _dotnetifyHub = __webpack_require__(14);

var _dotnetifyHub2 = _interopRequireDefault(_dotnetifyHub);

var _dotnetifyHubLocal = __webpack_require__(7);

var _dotnetifyHubLocal2 = _interopRequireDefault(_dotnetifyHubLocal);

var _dotnetifyHubWebapi = __webpack_require__(6);

var _dotnetifyHubWebapi2 = _interopRequireDefault(_dotnetifyHubWebapi);

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

function _toConsumableArray(arr) { if (Array.isArray(arr)) { for (var i = 0, arr2 = Array(arr.length); i < arr.length; i++) { arr2[i] = arr[i]; } return arr2; } else { return Array.from(arr); } }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

var dotnetifyFactory = exports.dotnetifyFactory = function () {
  function dotnetifyFactory() {
    _classCallCheck(this, dotnetifyFactory);
  }

  _createClass(dotnetifyFactory, null, [{
    key: 'create',
    value: function create() {
      var dotnetify = {
        // SignalR hub options.
        hub: _dotnetifyHub2.default,
        hubOptions: {
          transport: ['webSockets', 'longPolling'],

          // Use this to add customize HubConnectionBuilder.
          connectionBuilder: function connectionBuilder(builder) {
            return builder;
          }
        },
        hubPath: null,

        // Debug mode.
        debug: false,
        debugFn: null,

        // Offline mode. (WIP)
        offline: false,
        isOffline: true,
        offlineTimeout: 5000,
        offlineCacheFn: null,

        // Internal variables.
        _vmAccessors: [],

        // Use this to get notified of connection state changed events.
        // (state, exception, hub) => void
        connectionStateHandler: null,

        // Use this intercept a view model prior to establishing connection,
        // with the option to provide any connect parameters.
        // ({vmId, options}) => {vmId, options, hub}
        connectHandler: null,

        // Support changing hub server URL after first init.
        get hubServerUrl() {
          return this.hub.url;
        },

        set hubServerUrl(url) {
          this.hub.url = url;
          if (this.debug) console.log('SignalR: connecting to ' + this.hubServerUrl);
          if (this.hub.isHubStarted) this.startHub(this.hub, true);
        },

        // Generic connect function for non-React app.
        connect: function connect(iVMId, iOptions) {
          return dotnetify.react.connect(iVMId, {}, iOptions);
        },


        // Creates a SignalR hub client.
        createHub: function createHub(hubServerUrl, hubPath, hubLib) {
          return this.initHub(_dotnetifyHub.dotnetifyHubFactory.create(), hubPath, hubServerUrl, hubLib);
        },


        // Creates a Web API hub client.
        createWebApiHub: function createWebApiHub(baseUrl, onRequestHandler) {
          return (0, _dotnetifyHubWebapi.createWebApiHub)(baseUrl, onRequestHandler);
        },


        // Configures hub connection to SignalR hub server.
        initHub: function initHub(hub, hubPath, hubServerUrl, hubLib) {
          var _this = this;

          hub = hub || this.hub;
          hubPath = hubPath || this.hubPath;
          hubServerUrl = hubServerUrl || this.hubServerUrl;
          hubLib = hubLib || this.hubLib;

          if (!hub.isHubStarted) {
            hub.init(hubPath, hubServerUrl, hubLib);

            // Use SignalR event to raise the connection state event.
            hub.stateChanged(function (state) {
              return _this.handleConnectionStateChanged(state, null, hub);
            });
          }
          return hub;
        },


        // Used by a view to select a hub, and provides the opportunity to override any connect info.
        selectHub: function selectHub(vmConnectArgs) {
          vmConnectArgs = vmConnectArgs || {};
          vmConnectArgs.options = vmConnectArgs.options || {};
          var override = typeof this.connectHandler == 'function' && this.connectHandler(vmConnectArgs) || {};
          if (!override.hub) {
            override.hub = (0, _dotnetifyHubLocal.hasLocalVM)(vmConnectArgs.vmId) ? _dotnetifyHubLocal2.default : vmConnectArgs.options.webApi ? _dotnetifyHubWebapi2.default : this.initHub();
            override.hub.debug = this.debug;
          }
          return Object.assign(vmConnectArgs, override);
        },


        // Starts hub connection to SignalR hub server.
        startHub: function startHub(hub, forceRestart) {
          var _this2 = this;

          hub = hub || this.hub;

          var doneHandler = function doneHandler() {};
          var failHandler = function failHandler(ex) {
            return _this2.handleConnectionStateChanged('error', ex, hub);
          };
          hub.startHub(this.hubOptions, doneHandler, failHandler, forceRestart);
        },


        // Used by dotnetify-react and -vue to expose their view model accessors.
        addVMAccessor: function addVMAccessor(vmAccessor) {
          !this._vmAccessors.includes(vmAccessor) && this._vmAccessors.push(vmAccessor);
        },
        checkServerSideException: function checkServerSideException(iVMId, iVMData, iExceptionHandler) {
          var vmData = JSON.parse(iVMData);
          if (vmData && vmData.hasOwnProperty('ExceptionType') && vmData.hasOwnProperty('Message')) {
            var exception = { name: vmData.ExceptionType, message: vmData.Message };

            if (typeof iExceptionHandler === 'function') {
              return iExceptionHandler(exception);
            } else {
              console.error('[' + iVMId + '] ' + exception.name + ': ' + exception.message);
              throw exception;
            }
          }
        },


        // Get all view models.
        getViewModels: function getViewModels() {
          return this._vmAccessors.reduce(function (prev, current) {
            return [].concat(_toConsumableArray(prev), _toConsumableArray(current()));
          }, []).filter(function (val, idx, self) {
            return self.indexOf(val) === idx;
          }); // returns distinct items.
        },
        handleConnectionStateChanged: function handleConnectionStateChanged(iState, iException, iHub) {
          if (this.debug) console.log('SignalR: ' + (iException ? iException.message : iState));
          if (typeof this.connectionStateHandler === 'function') this.connectionStateHandler(iState, iException, iHub);else if (iException) console.error(iException);
        }
      };

      return dotnetify;
    }
  }]);

  return dotnetifyFactory;
}();

exports.default = dotnetifyFactory.create();

/***/ })
/******/ ]);
});
//# sourceMappingURL=dotnetify.js.map
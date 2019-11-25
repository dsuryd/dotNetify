(function webpackUniversalModuleDefinition(root, factory) {
	if(typeof exports === 'object' && typeof module === 'object')
		module.exports = factory(require("knockout"), require("jquery"), require("@aspnet/signalr"));
	else if(typeof define === 'function' && define.amd)
		define(["knockout", "jquery", "@aspnet/signalr"], factory);
	else if(typeof exports === 'object')
		exports["dotnetify"] = factory(require("knockout"), require("jquery"), require("@aspnet/signalr"));
	else
		root["dotnetify"] = factory(root["ko"], root["jQuery"], root["signalR"]);
})(window, function(__WEBPACK_EXTERNAL_MODULE__2__, __WEBPACK_EXTERNAL_MODULE__3__, __WEBPACK_EXTERNAL_MODULE__15__) {
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
/******/ 	return __webpack_require__(__webpack_require__.s = 23);
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
/* 2 */
/***/ (function(module, exports) {

module.exports = __WEBPACK_EXTERNAL_MODULE__2__;

/***/ }),
/* 3 */
/***/ (function(module, exports) {

module.exports = __WEBPACK_EXTERNAL_MODULE__3__;

/***/ }),
/* 4 */
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
/* 5 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
/* WEBPACK VAR INJECTION */(function(global) {

Object.defineProperty(exports, "__esModule", {
  value: true
});

var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) { return typeof obj; } : function (obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; /* 
                                                                                                                                                                                                                                                                              Copyright 2015-2018 Dicky Suryadi
                                                                                                                                                                                                                                                                              
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


var _dotnetify2 = __webpack_require__(22);

var _dotnetify3 = _interopRequireDefault(_dotnetify2);

var _jquery = __webpack_require__(3);

var _jquery2 = _interopRequireDefault(_jquery);

__webpack_require__(12);

var _knockout = __webpack_require__(2);

var ko = _interopRequireWildcard(_knockout);

function _interopRequireWildcard(obj) { if (obj && obj.__esModule) { return obj; } else { var newObj = {}; if (obj != null) { for (var key in obj) { if (Object.prototype.hasOwnProperty.call(obj, key)) newObj[key] = obj[key]; } } newObj.default = obj; return newObj; } }

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

ko.mapping = __webpack_require__(11);

var window = window || global || {};
var dotnetify = window.dotnetify || _dotnetify3.default;

dotnetify.ko = {
  version: '2.0.0',
  controller: dotnetify,

  // Internal variables.
  _responseSubs: null,
  _connectedSubs: null,

  _responseVM: function _responseVM(iVMId, iVMData) {
    // Construct a selector from iVMId to find the associated widget.
    // First parse the instance Id out of the string, if present.
    var vmType = iVMId;
    var vmInstanceId = null;
    if (vmType.indexOf('$') >= 0) {
      var path = iVMId.split('$');
      vmType = path[0];
      vmInstanceId = path[1];
    }

    var selector = "[data-vm='" + vmType + "']";

    // If present, add the master view models to the selector.
    var path = vmType.split('.');
    if (path.length > 1) {
      selector = '';
      var i = 0;
      for (i = 0; i < path.length - 1; i++) {
        selector += "[data-master-vm='" + path[i] + "'] ";
      }selector += "[data-vm='" + path[i] + "']";
    }

    // If present, add the instance Id to the selector.
    if (vmInstanceId != null) selector += "[data-vm-id='" + vmInstanceId + "']";

    // Use the selector to locate the view model widget and pass the data.
    var element = (0, _jquery2.default)(selector);
    if (element.length > 0) {
      dotnetify.checkServerSideException(iVMId, iVMData);
      element.data('ko-dotnetify').UpdateVM(iVMData);
      return true;
    }
    return false;
  },

  init: function init() {
    var self = dotnetify.ko;
    var hub = dotnetify.selectHub().hub;

    var applyWidget = function applyWidget() {
      _jquery2.default.each((0, _jquery2.default)('[data-vm]'), function () {
        (0, _jquery2.default)(this).dotnetify();
      });
    };

    if (!self._responseSubs) self._responseSubs = hub.responseEvent.subscribe(self._responseVM);

    if (!self._connectedSubs) self._connectedSubs = hub.connectedEvent.subscribe(function () {
      applyWidget();
    });

    // If offline mode is enabled, apply the widget anyway when there's no connection.
    setTimeout(function () {
      if (dotnetify.offline && !hub.isConnected) {
        applyWidget();
        dotnetify.isOffline = true;
        (0, _jquery2.default)(document).trigger('offline', dotnetify.isOffline);
      }
    }, dotnetify.offlineTimeout);

    dotnetify.startHub(hub);

    // Use SignalR event to raise the offline event with true/false argument.
    hub.stateChanged(function (state) {
      if (dotnetify.debug) console.log('SignalR: ' + state);

      var isOffline = state != 'connected';
      if (dotnetify.isOffline != isOffline) {
        dotnetify.isOffline = isOffline;
        (0, _jquery2.default)(document).trigger('offline', dotnetify.isOffline);
      }
    });

    if (dotnetify.offline) applyWidget();
  },

  destroy: function destroy(iParent) {
    var elems = iParent ? (0, _jquery2.default)(iParent).find('[data-vm]').addBack(iParent) : (0, _jquery2.default)('[data-vm]');
    elems.toArray().forEach(function (elem) {
      var widget = dotnetify.ko.widget(elem);
      if (widget) widget.destroy();
    });
  },

  // Get all view models.
  getViewModels: function getViewModels() {
    var self = dotnetify.ko;
    var elems = (0, _jquery2.default)('[data-vm]').toArray();
    return elems.map(function (elem) {
      var widget = dotnetify.ko.widget(elem);
      return widget ? widget.VM : null;
    }).filter(function (x) {
      return x;
    });
  },

  widget: function widget(iElement) {
    return (0, _jquery2.default)(iElement).data('ko-dotnetify');
  },

  plugins: {}
};

(0, _jquery2.default)(function () {
  dotnetify.ko.init();
});

_jquery2.default.widget('ko.dotnetify', {
  // Widget constructor.
  _create: function _create() {
    var self = this;

    self.VMType = self.element.attr('data-vm');
    self.VMId = self.VMType;

    // If an instance Id is specified, add it to VMId.
    var instanceId = self.element.attr('data-vm-id');
    if (instanceId != null) self.VMId += '$' + instanceId;

    // If inside master view scope, combine the names into VMId.
    _jquery2.default.each(self.element.parents('[data-master-vm]'), function () {
      self.VMId = (0, _jquery2.default)(this).attr('data-master-vm') + '.' + self.VMId;
    });

    // Handle offline mode.
    if (dotnetify.offline) self._ListenToOfflineEvent();

    // Request the server VM.
    if (self.VMId != null) {
      if (dotnetify.hub.isConnected) self._RequestVM();else if (dotnetify.offline) self._GetOfflineVM();
    } else console.error("ERROR: dotnetify - failed to find 'data-vm' attribute in the element where .dotnetify() was applied.");
  },

  // Widget destructor.
  _destroy: function _destroy() {
    try {
      var self = this;

      // Stop listening to offline event.
      if (typeof self.OfflineFn === 'function') (0, _jquery2.default)(document).off('offline', self.OfflineFn);

      // Call any plugin's $destroy function if provided.
      _jquery2.default.each(dotnetify.plugins, function (pluginId, plugin) {
        if (typeof plugin['$destroy'] === 'function') plugin.$destroy.apply(self.VM);
      });

      // Call view model's $destroy function if provided.
      if (self.VM != null && self.VM.hasOwnProperty('$destroy')) self.VM.$destroy();
    } catch (e) {
      console.error(e.stack);
    }

    dotnetify.hub.disposeVM(self.VMId);
  },

  // Convert the server VM into knockout VM.
  UpdateVM: function UpdateVM(iVMData) {
    var self = this;
    try {
      // If no view model yet, create one from the server data.
      if (self.VM == null) {
        self.VM = ko.mapping.fromJS(JSON.parse(iVMData));

        // Set essential info to the view model.
        self.VM.$vmId = self.VMId;
        self.VM.$element = self.element;
        self.VM.$dotnetify = dotnetify.ko;

        // Add an observable to carry the offline state.
        if (dotnetify.offline) self.VM.$vmOffline = ko.observable(self.IsOffline);

        // Add built-in functions to the view model.
        this._AddBuiltInFunctions();

        // Call any plugin's $init function if provided to give a chance to do
        // things before initial binding is applied.
        _jquery2.default.each(dotnetify.ko.plugins, function (pluginId, plugin) {
          if (typeof plugin['$init'] === 'function') plugin.$init.apply(self.VM);
        });

        // Call view model's init function if provided.
        if (typeof self.VM['$init'] === 'function') self.VM.$init();

        // Apply knockout view model to the HTML element.
        try {
          ko.applyBindings(self.VM, self.element[0]);
        } catch (e) {
          console.error(e.stack);
        }

        // Enable server update so that every changed value goes to server.
        self.VM.$serverUpdate = true;

        // Raise the ready event after all knockout components are ready.
        self._OnComponentReady(function () {
          // Call any plugin's $ready function if provided to give a chance to do
          // things when the view model is ready.
          _jquery2.default.each(dotnetify.ko.plugins, function (pluginId, plugin) {
            if (typeof plugin['$ready'] === 'function') plugin.$ready.apply(self.VM);
          });

          // Subscribe to change events to allow sending updates back to server.
          self._SubscribeObservables(self.VM);

          // Call view model's $ready function if provided.
          if (typeof self.VM['$ready'] === 'function') self.VM.$ready();

          // Send 'ready' event after a new view model was received.
          self.element.trigger('ready', { VMId: self.VMId, VM: self.VM });
        });

        // Cache the VM data in case of offline mode.
        if (dotnetify.offline && dotnetify.hub.isConnected && typeof dotnetify.offlineCacheFn === 'function') dotnetify.offlineCacheFn(self.VMId + self.element.attr('data-vm-arg'), iVMData);
      } else {
        // Disable server update because we're going to update the value in the knockout VM
        // and that will trigger change event back to server if we don't stop it now.
        self.VM.$serverUpdate = false;

        var vmUpdate = JSON.parse(iVMData);
        self._PreProcess(vmUpdate);

        try {
          ko.mapping.fromJS(vmUpdate, self.VM);
        } catch (e) {
          console.error(e.stack);
        }

        // Don't forget to re-enable sending changed values to server.
        self.VM.$serverUpdate = true;

        // Subscribe to change events to allow sending updates back to server,
        // but do it after all the knockout components are ready.
        self._OnComponentReady(function () {
          self._SubscribeObservables(self.VM);
        });
      }
    } catch (e) {
      console.error(e.stack);
    }

    if (dotnetify.debug) {
      console.log('[' + self.VMId + '] received> ');
      console.log(JSON.parse(iVMData));

      if (dotnetify.debugFn != null) dotnetify.debugFn(self.VMId, 'received', JSON.parse(iVMData));
    }
  },

  // Adds built-in functions to the view model.
  _AddBuiltInFunctions: function _AddBuiltInFunctions() {
    var self = this;

    // Executes the given function in a scope where server update is temporarily disabled.
    self.VM.$preventBinding = function (fn) {
      self.VM.$serverUpdate = false;
      fn.apply(self.VM);
      self.VM.$serverUpdate = true;
    };

    // Adds a new item to an observable array.
    self.VM.$addList = function (iList, iNewItem) {
      var newItem = Array.isArray(iNewItem) ? iNewItem : ko.mapping.fromJS(iNewItem);

      // Check if the list already has an item with the same key. If so, replace it.
      var key = iList()['$vmKey'];
      if (key != null) {
        var match = ko.utils.arrayFirst(iList(), function (i) {
          return i[key]() == newItem[key]();
        });
        if (match != null) {
          iList.replace(match, newItem);
          return;
        }
      }
      iList.push(newItem);
    };

    // Updates existing item to an observable array.
    self.VM.$updateList = function (iList, iNewItem) {
      var newItem = ko.mapping.fromJS(iNewItem);

      // Check if the list already has an item with the same key. If so, update it.
      var key = iList()['$vmKey'];
      if (key != null) {
        if (!newItem.hasOwnProperty(key)) {
          console.error("ERROR: object requires property '" + key + "'");
          return;
        }
        var match = ko.utils.arrayFirst(iList(), function (i) {
          return i[key]() == newItem[key]();
        });
        if (match != null) {
          Object.keys(newItem).forEach(function (prop) {
            if (ko.isObservable(newItem[prop])) match[prop](newItem[prop]());
          });

          return;
        }
      }
      iList.push(newItem);
    };

    // Removes an item from an observable array.
    // Unlike the push operation, the ko remove operation will cause the list to trigger
    // change event, therefore disable server update while we do this.
    self.VM.$removeList = function (iList, iCriteria) {
      self.VM.$preventBinding(function () {
        iList.remove(iCriteria);
      });
    };

    // Listens to a view model property change event.
    self.VM.$on = function (iProperty, iCallback) {
      iProperty.subscribe(function (iNewValue) {
        iCallback(iNewValue);
      });
    };

    // Listens to a view model property change event once.
    self.VM.$once = function (iProperty, iCallback) {
      var subscription = iProperty.subscribe(function (iNewValue) {
        subscription.dispose();
        iCallback(iNewValue);
      });
    };

    // Loads a view into a target element.
    // Method parameters: TargetSelector, ViewUrl, [iJsModuleUrl], [iVmArg], iCallbackFn
    self.VM.$loadView = function (iTargetSelector, iViewUrl, iJsModuleUrl, iVmArg, iCallbackFn) {
      if ((typeof iJsModuleUrl === 'undefined' ? 'undefined' : _typeof(iJsModuleUrl)) === 'object' && iJsModuleUrl != null) {
        iCallbackFn = iVmArg;
        iVmArg = iJsModuleUrl;
        iJsModuleUrl = null;
      } else if (typeof iJsModuleUrl === 'function') {
        iCallbackFn = iJsModuleUrl;
        iJsModuleUrl = null;
      } else if (typeof iVmArg === 'function') {
        iCallbackFn = iVmArg;
        iVmArg = null;
      }

      // If no view URL is given, empty the target DOM element.
      if (iViewUrl == null || iViewUrl == '') {
        (0, _jquery2.default)(iTargetSelector).empty();
        return;
      }

      // Loads the view template to the target DOM element.
      (0, _jquery2.default)(iTargetSelector).load(iViewUrl, null, function () {
        // Adds view model arguments when provided.
        if (iVmArg != null && !_jquery2.default.isEmptyObject(iVmArg)) (0, _jquery2.default)(this).attr('data-vm-arg', JSON.stringify(iVmArg));

        // Call the callback function.
        if (typeof iCallbackFn === 'function') iCallbackFn.apply(this);

        // Load the Javascript module if specified.
        if (iJsModuleUrl != null) {
          _jquery2.default.getScript(iJsModuleUrl, function () {
            dotnetify.ko.init();
          });
        } else dotnetify.ko.init();
      });
    };

    // Injects a context with observables mapped from an object. Context can be an object or an observable array.
    self.VM.$inject = function (iContext, iObject) {
      if (ko.isObservable(iContext) && 'push' in iContext) _jquery2.default.each(iContext(), function (idx, item) {
        self._Inject(item, iObject);
      });else self._Inject(iContext, iObject);
    };

    // Map the module in global namespace whose name matches the view model type.
    var jsModule = window[self.VMType];
    if (jsModule != null) {
      // If the module is a Typescript class, instantiate it.
      if (typeof jsModule === 'function') {
        var jsInstance = new jsModule(self.VM);
        Object.assign(jsInstance, jsModule.prototype);
        self._Inject(self.VM, jsInstance);
      } else self._Inject(self.VM, jsModule);
    }

    // Add plugin functions.
    _jquery2.default.each(dotnetify.ko.plugins, function (pluginId, plugin) {
      if (plugin.hasOwnProperty('$inject')) plugin.$inject(self.VM);
    });
  },

  // Gets offline view model data from the local cache.
  _GetOfflineVM: function _GetOfflineVM() {
    var self = this;

    if (typeof dotnetify.offlineCacheFn === 'function') {
      // SignalR connection isn't available; use cached VM data for offline mode.
      var cachedData = dotnetify.offlineCacheFn(self.VMId + self.element.attr('data-vm-arg'));
      if (cachedData == null) cachedData = dotnetify.offlineCacheFn(self.VMId);

      if (cachedData != null) {
        if (dotnetify.debug) console.warn('[' + self.VMId + '] using offline data');

        self.IsOffline = true;
        self.UpdateVM(cachedData);
      }
    }
  },

  // Initializes offline mode handling.
  _ListenToOfflineEvent: function _ListenToOfflineEvent() {
    var self = this;

    self.IsOffline = false;
    self.OfflineFn = function (event, isOffline) {
      if (self.VM != null && self.VM.hasOwnProperty('$vmOffline')) self.VM.$vmOffline(isOffline);

      self.IsOffline = isOffline;
      if (!isOffline) self._RequestVM();else if (self.VM == null) self._GetOfflineVM();

      (0, _jquery2.default)(document).one('offline', self.OfflineFn.bind(self));
    };

    (0, _jquery2.default)(document).one('offline', self.OfflineFn.bind(self));
  },

  // Inject the context with observables mapped from an object.
  // Properties that start with underscore are mapped to observables.
  // Functions that start with underscore are mapped to pure computed observables.
  _Inject: function _Inject(iContext, iObject) {
    Object.keys(iObject).forEach(function (prop) {
      // Skip if the context already has a property with the same name.
      if (iContext.hasOwnProperty(prop)) return;

      if (typeof iObject[prop] === 'function') {
        if (prop.indexOf('_') == 0) {
          iContext[prop] = ko.pureComputed(iObject[prop], iContext);
        } else iContext[prop] = iObject[prop];
      } else if (prop.indexOf('_') == 0) {
        iContext[prop] = ko.observable(iObject[prop]);

        // Prevent it from being subscribed so it won't get sent to server.
        iContext[prop].$subscribe = true;
      } else iContext[prop] = iObject[prop];
    });
  },

  // Calls the callback function only if all the knockout components are ready.
  // This is a workaround until knockout issue #1533 is closed.
  _OnComponentReady: function _OnComponentReady(iCallbackFn) {
    var self = this;
    var retry = 0;
    var checkComponentReadyFn = function checkComponentReadyFn() {
      var isReady = true;

      // Assume the knockout components are those with 'params' attribute,
      // and that it's ready if it has at least one child element.
      var components = self.element.find('[params]');
      if (components.length > 0) isReady = _jquery2.default.grep(components, function (i) {
        return i.childElementCount == 0;
      }).length == 0;

      if (isReady || retry++ > 3) iCallbackFn();else setTimeout(checkComponentReadyFn, 250);
    };

    checkComponentReadyFn();
  },

  // On value changed from a knockout VM's observable, update the server VM.
  _OnValueChanged: function _OnValueChanged(iVMPath, iNewValue) {
    var update = {};
    update[iVMPath] = iNewValue instanceof Object ? _jquery2.default.extend({}, iNewValue) : iNewValue;

    if (dotnetify.hub.isConnected) {
      try {
        dotnetify.hub.updateVM(this.VMId, update);

        if (dotnetify.debug) {
          console.log('[' + this.VMId + '] sent> ');
          console.log(update);

          if (dotnetify.debugFn != null) dotnetify.debugFn(this.VMId, 'sent', update);
        }
      } catch (e) {
        console.error(e);
      }
    }
  },

  // Preprocess view model update from the server before we map it to knockout view model.
  _PreProcess: function _PreProcess(iVMUpdate) {
    var _this = this;

    Object.keys(iVMUpdate).forEach(function (prop) {
      // Look for property that end with '_add'. Interpret the value as a list item to be added
      // to an existing list whose property name precedes that suffix.
      var match = /(.*)_add/.exec(prop);
      if (match != null) {
        var list = _this.VM[match[1]];
        if (list != null) _this.VM.$addList(list, iVMUpdate[prop]);else throw new Error('unable to resolve ' + prop);
        delete iVMUpdate[prop];
        return;
      }

      // Look for property that end with '_update'. Interpret the value as a list item to be updated
      // to an existing list whose property name precedes that suffix.
      var match = /(.*)_update/.exec(prop);
      if (match != null) {
        var list = _this.VM[match[1]];
        if (list != null) _this.VM.$updateList(list, iVMUpdate[prop]);else throw new Error('unable to resolve ' + prop);
        delete iVMUpdate[prop];
        return;
      }

      // Look for property that end with '_remove'. Interpret the value as a list item key to remove
      // from an existing list whose property name precedes that suffix.
      var match = /(.*)_remove/.exec(prop);
      if (match != null) {
        var list = _this.VM[match[1]];
        if (list != null) {
          var key = list()['$vmKey'];
          if (key != null) _this.VM.$removeList(_this.VM[match[1]], function (i) {
            return i[key]() == iVMUpdate[prop];
          });else throw new Error('unable to resolve ' + prop + ' due to missing vmItemKey attribute');
        } else throw new Error('unable to resolve ' + prop);
        delete iVMUpdate[prop];
        return;
      }
    });
  },

  // Requests view model data from the server.
  _RequestVM: function _RequestVM() {
    var self = this;
    var vmArg = self.element.attr('data-vm-arg');
    vmArg = vmArg != null ? _jquery2.default.parseJSON(vmArg.replace(/'/g, '"')) : null;

    if (dotnetify.hub.isConnected) {
      try {
        dotnetify.hub.requestVM(self.VMId, vmArg);
      } catch (e) {
        console.error(e);
      }
    }
  },

  // Subscribe to value change events raised by knockout VM's observables.
  _SubscribeObservables: function _SubscribeObservables(iParam, iVMPath) {
    var _this2 = this;

    var self = this;

    if (iParam == null) return;else if (ko.isObservable(iParam)) {
      if ('$subscribe' in iParam === false) {
        iParam.subscribe(function (iNewValue) {
          // Handle value change event from observables.
          if (self.VM.$serverUpdate === true) self._OnValueChanged(iVMPath, iNewValue);
        });
        iParam['$subscribe'] = true;
      }
      this._SubscribeObservables(iParam(), iVMPath);
    } else if ((typeof iParam === 'undefined' ? 'undefined' : _typeof(iParam)) == 'object') {
      // The property with $vmKey means it's an enumerable and the $vmKey indicates the key to identify
      // the item in that enumerable.  When we send value update to the server, we'll use the property
      // path in this format: <enumerable property name>.$<key value>.<property name>.
      // For example: ListContent.$3.FirstName.
      var key = '$vmKey' in iParam ? iParam['$vmKey'] : null;

      Object.keys(iParam).forEach(function (property) {
        if (property.charAt(0) == '$' || property.charAt(0) == '_') return;
        if (property.charAt(0) == property.charAt(0).toLowerCase()) return;
        if (!isNaN(property.charAt(0))) return;
        var path = key != null ? '$' + iParam[property][key]() : property;
        _this2._SubscribeObservables(iParam[property], iVMPath == null ? path : iVMPath + '.' + path);
      });
    } else if (iParam instanceof Array) {
      Object.keys(iParam).forEach(function (index) {
        path = '$' + index;
        _this2._SubscribeObservables(iParam[index], iVMPath == null ? path : iVMPath + '.' + path);
      });
    }
  }
});

// Custom knockout binding to indicate the item key of an items collection property.
ko.bindingHandlers.vmItemKey = {
  preprocess: function preprocess(value) {
    // Make sure the item key is enclosed with quotes.
    return value.charAt(0) != "'" ? "'" + value + "'" : value;
  },
  update: function update(element, valueAccessor, allBindings, viewModel, bindingContext) {
    var value = valueAccessor();
    var items = allBindings.get('foreach');

    // Test whether the foreach value is object literal where items is set to 'data' property.
    if (!ko.isObservable(items) && items.hasOwnProperty('data')) items = items.data;

    // Store the item key in a special property '$vmKey' in the element's view model.
    if (ko.isObservable(items) && items() != null && value != null) items()['$vmKey'] = value;
  }
};

// Custom knockout binding to bind the specified function the click event of the element.
ko.bindingHandlers.vmCommand = {
  init: function init(element, valueAccessor, allBindings, viewModel, bindingContext) {
    var vm = bindingContext.$root;
    var fnName = null;
    var fnArg = null;

    // Parse the value. It supports either a function name or an object literal { funcName: argument }
    // where argument can be either data or obsevables.
    var matchFnNameArg = /return\s{(.*):(.*)}\s/.exec(valueAccessor.toString());
    if (matchFnNameArg != null) {
      fnName = matchFnNameArg[1].trim();
      fnArg = matchFnNameArg[2].trim();
    } else {
      var matchFnName = /return\s(.*)\s/.exec(valueAccessor.toString());
      if (matchFnName != null) fnName = matchFnName[1].trim();
    }

    if (fnName == null) throw new Error('invalid vmCommand value at ' + element.outerHTML);

    // Support whether function is defined globally or inside a namespace that matches view model Id.
    var getFn = function getFn() {
      return vm[fnName] != null ? vm[fnName] : valueAccessor();
    };

    // Trim the argument from enclosing quotes.  If it's an observable name, replace it with the value.
    if (fnArg != null) {
      if (fnArg.charAt(0) == "'") fnArg = fnArg.replace(/(^'|'$)/g, '');else if (ko.isObservable(viewModel[fnArg])) fnArg = ko.unwrap(viewModel[fnArg]);else if (fnArg == '$data') fnArg = viewModel;
    } else fnArg = true;

    var newValueAccessor = function newValueAccessor() {
      return function () {
        var fn = getFn();

        // If function is an observable, which means it's a server view model property, then set its value to
        // trigger the invocation of its setter property on the server side.  If it's not an observable, then it must
        // be a client-side function, in which case just invoke it and pass all possible objects it may need.
        if (ko.isObservable(fn)) {
          // Reset the value locally first to ensure that setting the value will raise change events.
          vm.$preventBinding(function () {
            fn(fnArg === true ? false : null);
          });

          fn(fnArg);
        } else fn.apply(vm, [viewModel, element, bindingContext.$parent]);
      };
    };
    ko.bindingHandlers.click.init(element, newValueAccessor, allBindings, viewModel, bindingContext);
  }
};

// Custom knockout binding to call a function on initial property change event.
ko.bindingHandlers.vmOnce = {
  init: function init(element, valueAccessor, allBindings, viewModel, bindingContext) {
    ko.bindingHandlers.vmOn.init(element, valueAccessor, allBindings, viewModel, bindingContext, true);
  }
};

// Custom knockout binding to call a function on a property change event.
ko.bindingHandlers.vmOn = {
  init: function init(element, valueAccessor, allBindings, viewModel, bindingContext, once) {
    var vm = bindingContext.$root;
    var property = null;
    var fnName = null;
    var value = valueAccessor.toString();

    // Parse the value, which should be in object literal { property: fnName }.
    var match = /return\s{(.*):(.*)}\s/.exec(value);
    if (match != null) {
      property = match[1].trim();
      fnName = match[2].trim();
    }

    if (fnName == null) throw new Error('invalid vmOn function at ' + element.outerHTML);

    // Support whether function is defined globally or inside a namespace that matches view model Id.
    var getFn = function getFn() {
      return vm[fnName] != null ? vm[fnName] : valueAccessor()[property];
    };

    // Make sure the property is an observable.
    if (property != null && !ko.isObservable(viewModel[property])) throw new Error('invalid vmOn data: ' + valueAccessor());

    // Call the function with the initial data.
    getFn().apply(vm, [viewModel, element, bindingContext.$parent]);

    // Call the function on every data update.
    if (once == null) viewModel[property].subscribe(function (iNewValue) {
      getFn().apply(vm, [viewModel, element, bindingContext.$parent]);
    });
  }
};

exports.default = dotnetify;
/* WEBPACK VAR INJECTION */}.call(this, __webpack_require__(0)))

/***/ }),
/* 6 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


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


var _utils = __webpack_require__(1);

var _utils2 = _interopRequireDefault(_utils);

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

function _toConsumableArray(arr) { if (Array.isArray(arr)) { for (var i = 0, arr2 = Array(arr.length); i < arr.length; i++) { arr2[i] = arr[i]; } return arr2; } else { return Array.from(arr); } }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

var dotnetifyVMRouter = function () {
  _createClass(dotnetifyVMRouter, [{
    key: 'RoutingState',
    get: function get() {
      throw new Error('Not implemented');
    }
  }, {
    key: 'VMRoot',
    get: function get() {
      throw new Error('Not implemented');
    }
  }, {
    key: 'VMArg',
    get: function get() {
      throw new Error('Not implemented');
    }
  }]);

  function dotnetifyVMRouter(vm, router) {
    _classCallCheck(this, dotnetifyVMRouter);

    this.routes = [];

    this.vm = vm;
    this.router = router;
    this.debug = vm.$dotnetify.controller.debug;
  }

  // Dispatch the active routing state to the server.


  _createClass(dotnetifyVMRouter, [{
    key: 'dispatchActiveRoutingState',
    value: function dispatchActiveRoutingState(iPath) {
      this.vm.$dispatch({ 'RoutingState.Active': iPath });

      var _vm$State = this.vm.State(),
          RoutingState = _vm$State.RoutingState;

      RoutingState = Object.assign(RoutingState || {}, { Active: iPath });
      this.vm.State({ RoutingState: RoutingState });
    }

    // Handles click event from anchor tags.  Argument can be event object or path string.

  }, {
    key: 'handleRoute',
    value: function handleRoute(iArg) {
      var _this = this;

      var path = null;
      if ((typeof iArg === 'undefined' ? 'undefined' : _typeof(iArg)) === 'object') {
        iArg.preventDefault();
        path = iArg.currentTarget.pathname;
      } else if (typeof iArg === 'string') path = iArg;

      if (path == null || path == '') throw new Error('$handleRoute requires path argument or event with pathname.');
      setTimeout(function () {
        return _this.router.pushState({}, '', path);
      });
    }

    // Build the absolute root path from the "vmRoot" property on React component.

  }, {
    key: 'initRoot',
    value: function initRoot() {
      if (!this.hasRoutingState || this.RoutingState === null || this.RoutingState.Root === null) return;

      if (this._absRoot != this.RoutingState.Root) {
        var absRoot = _utils2.default.trim(this.VMRoot);
        if (absRoot != '') absRoot = '/' + absRoot;
        var root = _utils2.default.trim(this.RoutingState.Root);
        this._absRoot = root != '' ? absRoot + '/' + root : absRoot;
        this.RoutingState.Root = this._absRoot;
      }
    }

    // Initialize routing templates if the view model implements IRoutable.

  }, {
    key: 'initRouting',
    value: function initRouting() {
      var _this2 = this;

      var vm = this.vm;
      if (!this.hasRoutingState) return;

      if (this.RoutingState === null) {
        console.error("router> the RoutingState prop of '" + vm.$vmId + "' was not initialized.");
        return;
      }

      var templates = this.RoutingState.Templates;
      if (templates == null || templates.length == 0) return;

      // Initialize the router.
      if (!this.router.$init) {
        this.router.init();
        this.router.$init = true;
      }

      // Build the absolute root path.
      this.initRoot();

      templates.forEach(function (template) {
        // If url pattern isn't given, consider Id as the pattern.
        var urlPattern = template.UrlPattern != null ? template.UrlPattern : template.Id;
        urlPattern = urlPattern != '' ? urlPattern : '/';
        var mapUrl = _this2.toUrl(urlPattern);

        if (_this2.debug) console.log('router> map ' + mapUrl + ' to template id=' + template.Id);

        _this2.router.mapTo(mapUrl, function (iParams) {
          _this2.router.urlPath = '';

          // Construct the path from the template pattern and the params passed by PathJS.
          var path = urlPattern;
          for (var param in iParams) {
            path = path.replace(':' + param, iParams[param]);
          }path = path.replace(/\(\/:([^)]+)\)/g, '').replace(/\(|\)/g, '');

          _this2.routeTo(path, template);
        });
      });

      // Route initial URL.
      var activeUrl = this.toUrl(this.RoutingState.Active);
      if (this.router.urlPath == '') this.router.urlPath = activeUrl;
      if (!this.routeUrl())
        // If routing ends incomplete, raise routed event anyway.
        this.raiseRoutedEvent(true);
    }

    // Whether a route is active.

  }, {
    key: 'isActive',
    value: function isActive(iRoute) {
      if (iRoute != null && iRoute.hasOwnProperty('Path')) {
        return _utils2.default.equal(iRoute.Path, this.RoutingState.Active);
      }
      return false;
    }

    // Loads an HTML view.

  }, {
    key: 'loadHtmlView',
    value: function loadHtmlView(iTargetSelector, iViewUrl, iJsModuleUrl, iCallbackFn) {
      var vm = this.vm;
      this.unmountView(iTargetSelector);

      // Load the HTML view.
      $(iTargetSelector).load(iViewUrl, null, function () {
        if (iJsModuleUrl != null) {
          var getScripts = iJsModuleUrl.split(',').map(function (i) {
            return $.getScript(i);
          });
          $.when.apply($, getScripts).done(function () {
            return typeof callbackFn === 'function' && iCallbackFn.call(vm);
          });
        } else if (typeof callbackFn === 'function') iCallbackFn.call(vm);
      });
    }
  }, {
    key: 'loadHtmlElementView',
    value: function loadHtmlElementView(iTargetSelector, iHtmlElement, iJsModuleUrl, iVmArg, iCallbackFn) {
      var _this3 = this;

      var vm = this.vm;
      var mountViewFunc = function mountViewFunc() {
        _this3.unmountView(iTargetSelector);

        var target = document.querySelector(iTargetSelector);
        while (target.firstChild) {
          target.removeChild(target.firstChild);
        }target.appendChild(iHtmlElement);

        if (typeof callbackFn === 'function') iCallbackFn.call(vm);
      };

      if (iJsModuleUrl == null) mountViewFunc();else {
        // Load all javascripts first. Multiple files can be specified with comma delimiter.
        var getScripts = iJsModuleUrl.split(',').map(function (i) {
          return $.getScript(i);
        });
        $.when.apply($, getScripts).done(mountViewFunc);
      }
    }

    // Loads a view into a target element.

  }, {
    key: 'loadView',
    value: function loadView(iTargetSelector, iViewUrl, iJsModuleUrl, iVmArg, iCallbackFn) {
      throw new Error('Not implemented');
    }

    // Routes to a path.

  }, {
    key: 'manualRouteTo',
    value: function manualRouteTo(iPath, iTarget, iViewUrl, iJSModuleUrl) {
      var vm = this.vm;
      var template = { Id: 'manual', Target: iTarget, ViewUrl: iViewUrl, JSModuleUrl: iJSModuleUrl };
      this.routeTo(iPath, template, true);
    }

    // Handles route enter event.

  }, {
    key: 'onRouteEnter',
    value: function onRouteEnter(iPath, iTemplate) {
      return true;
    }

    // Raise event indicating the routing process has ended.

  }, {
    key: 'raiseRoutedEvent',
    value: function raiseRoutedEvent(force) {
      var vm = this.vm;
      if (this.router.urlPath == '' || force == true) {
        if (this.debug) console.log('router> routed');
        _utils2.default.dispatchEvent('dotnetify.routed');
      }
    }

    // Returns the URL for an anchor tag.

  }, {
    key: 'route',
    value: function route(iRoute, iTarget) {
      // No route to process. Return silently.
      if (iRoute == null) return;

      if (!iRoute.hasOwnProperty('Path') && !iRoute.hasOwnProperty('TemplateId')) throw new Error('Not a valid route');

      // Build the absolute root path.
      this.initRoot();

      // If the route path is not defined, use the URL pattern from the associated template.
      // This is so that we don't send the same data twice if both are equal.
      var path = iRoute.Path;
      var template = null;
      if (this.hasRoutingState && this.RoutingState.Templates != null && iRoute.TemplateId != null) {
        var match = this.RoutingState.Templates.filter(function (iTemplate) {
          return iTemplate.Id == iRoute.TemplateId;
        });
        if (match.length > 0) {
          template = match[0];

          if (typeof iTarget === 'string') template.Target = iTarget;

          if (path == null) {
            path = template.UrlPattern != null ? template.UrlPattern : template.Id;
            iRoute.Path = path;
          }
        } else if (iRoute.RedirectRoot == null) throw new Error('vmRoute cannot find route template ' + iRoute.TemplateId);
      }

      // If the path has a redirect root, the path doesn't belong to the current root and needs to be
      // redirected to a different one.  Set the absolute path to the HREF attribute, and prevent the
      // default behavior of the anchor click event and instead do push to HTML5 history state, which
      // would attempt to resolve the path first before resorting to hard browser redirect.
      if (iRoute.RedirectRoot != null) {
        // Combine the redirect root with the view model's root.
        var redirectRoot = iRoute.RedirectRoot;
        if (redirectRoot.charAt(0) == '/') redirectRoot = redirectRoot.substr(0, redirectRoot.length - 1);
        var redirectRootPath = iRoute.RedirectRoot.split('/');

        var urlRedirect = '';
        var absRoot = this.VMRoot;
        if (absRoot != null) {
          var absRootPath = absRoot.split('/');
          for (var i = 0; i < absRootPath.length; i++) {
            if (absRootPath[i] != '' && absRootPath[i] == redirectRootPath[0]) break;
            urlRedirect += absRootPath[i] + '/';
          }
        }

        urlRedirect += redirectRoot + '/' + path;
        urlRedirect = urlRedirect.replace(/\/\/+/g, '/');
        if (!this.routes.some(function (x) {
          return x.Path === path;
        })) this.routes.push({ Path: path, Url: urlRedirect });
        return urlRedirect;
      }

      // For quick lookup, save the mapping between the path to the route inside the view model.
      if (template == null) throw new Error('vmRoute cannot find any route template');

      iRoute.$template = template;
      this.pathToRoute = this.pathToRoute || {};
      this.pathToRoute[path] = iRoute;

      // Set the absolute path to the HREF attribute, and prevent the default behavior of
      // the anchor click event and instead do push to HTML5 history state.
      var url = this.toUrl(path);
      url = url.length > 0 ? url : '/';
      if (!this.routes.some(function (x) {
        return x.Path === path;
      })) this.routes.push({ Path: path, Url: url });
      return url;
    }

    // Routes to a path.

  }, {
    key: 'routeTo',
    value: function routeTo(iPath, iTemplate, iDisableEvent, iCallbackFn, isRedirect) {
      var _this4 = this;

      var vm = this.vm;
      var viewModels = vm.$dotnetify.getViewModels();

      if (this.debug) console.log("router> route '" + iPath + "' to template id=" + iTemplate.Id);

      // We can determine whether the view has already been loaded by matching the 'RoutingState.Origin' argument
      // on the existing view model inside that target selector with the path.
      for (var i = 0; i < viewModels.length; i++) {
        var vmOther = viewModels[i];
        var vmArg = vmOther.$router.VMArg;
        if (vmArg != null) {
          if (typeof vmArg['RoutingState.Origin'] === 'string' && _utils2.default.equal(vmArg['RoutingState.Origin'], iPath)) return;
        }
      }

      // Support enter interception.
      if (iDisableEvent != true && vm.hasOwnProperty('onRouteEnter')) {
        if (this.onRouteEnter(iPath, iTemplate) == false || vm.onRouteEnter(iPath, iTemplate) == false) return;
      }

      // Check if the route has valid target.
      if (iTemplate.Target == null) {
        console.error("router> the Target for template '" + iTemplate.Id + "' was not set.  Use vm.onRouteEnter() to set the target.");
        return;
      }

      // If target DOM element isn't found, redirect URL to the path.
      if (document.getElementById(iTemplate.Target) == null) {
        if (isRedirect === true) {
          if (this.debug) console.log("router> target '" + iTemplate.Target + "' not found in DOM");
          return;
        } else {
          if (this.debug) console.log("router> target '" + iTemplate.Target + "' not found in DOM, use redirect instead");
          return this.router.redirect(this.toUrl(iPath), [].concat(_toConsumableArray(viewModels), _toConsumableArray(vm.$dotnetify.controller.getViewModels())));
        }
      }

      // Load the view associated with the route asynchronously.
      this.loadView('#' + iTemplate.Target, iTemplate.ViewUrl, iTemplate.JSModuleUrl, { 'RoutingState.Origin': iPath }, function () {
        // If load is successful, update the active route.
        _this4.dispatchActiveRoutingState(iPath);

        // Support exit interception.
        if (iDisableEvent != true && vm.hasOwnProperty('onRouteExit')) vm.onRouteExit(iPath, iTemplate);

        if (typeof iCallbackFn === 'function') iCallbackFn.call(vm);
      });
    }
  }, {
    key: 'routeToRoute',
    value: function routeToRoute(iRoute) {
      var _this5 = this;

      var path = this.vm.$route(iRoute);
      if (path == null || path == '') throw new Error('The route passed to $routeTo is invalid.');

      setTimeout(function () {
        return _this5.router.pushState({}, '', path);
      });
    }

    // Routes the URL if the view model implements IRoutable.
    // Returns true if the view model handles the routing.

  }, {
    key: 'routeUrl',
    value: function routeUrl(redirectUrlPath) {
      var _this6 = this;

      if (!this.hasRoutingState) return false;

      var isRedirect = !!redirectUrlPath;
      var root = this.RoutingState.Root;
      if (root == null) return false;

      // Get the URL path to route.
      var urlPath = isRedirect ? redirectUrlPath : this.router.urlPath;

      if (this.debug) console.log('router> routing ' + urlPath);

      // If the URL path matches the root path of this view, use the template with a blank URL pattern if provided.
      if (_utils2.default.equal(urlPath, root) || _utils2.default.equal(urlPath, root + '/') || urlPath === '/') {
        var match = _utils2.default.grep(this.RoutingState.Templates, function (iTemplate) {
          return iTemplate.UrlPattern === '';
        });
        if (match.length > 0) {
          this.routeTo('', match[0], null, null, isRedirect);
          this.router.urlPath = '';
          this.raiseRoutedEvent();
          return true;
        }
        return false;
      }

      // If the URL path starts with the root path of this view, look at the next path and try to match it with the
      // anchor tags in this view that are bound with the vmRoute binding type.  If there is match, route to that path.
      root = root + '/';
      if (_utils2.default.startsWith(urlPath, root)) {
        var routeElem = null;
        var _match = _utils2.default.grep(this.routes, function (elem) {
          return _utils2.default.startsWith(urlPath + '/', elem.Url + '/');
        });
        if (_match.length > 0) {
          // If more than one match, find the best match.
          for (var i = 0; i < _match.length; i++) {
            if (routeElem == null || routeElem.Url.length < _match[i].Url.length) routeElem = _match[i];
          }
        }

        if (routeElem != null) {
          var path = routeElem.Path;
          var template = this.hasOwnProperty('pathToRoute') && this.pathToRoute.hasOwnProperty(path) ? this.pathToRoute[path].$template : null;
          if (template != null) {
            // If the URL path is completely routed, clear it.
            if (_utils2.default.equal(this.router.urlPath, this.toUrl(path))) this.router.urlPath = '';

            // If route's not already active, route to it.
            if (!_utils2.default.equal(this.RoutingState.Active, path)) {
              this.routeTo(path, template, false, function () {
                return _this6.raiseRoutedEvent();
              }, isRedirect);
            } else this.raiseRoutedEvent();
            return true;
          }
        } else if (this.router.match(urlPath)) {
          // If no vmRoute binding matches, try to match with any template's URL pattern.
          this.router.urlPath = '';
          this.raiseRoutedEvent();
          return true;
        }
      }
      return false;
    }

    // Builds an absolute URL from a path.

  }, {
    key: 'toUrl',
    value: function toUrl(iPath) {
      var path = _utils2.default.trim(iPath);
      if (path.charAt(0) != '(' && path != '') path = '/' + path;
      return this.hasRoutingState ? this.RoutingState.Root + path : iPath;
    }

    // Unmount a view if there's one on the target selector.

  }, {
    key: 'unmountView',
    value: function unmountView(iTargetSelector) {
      throw new Error('Not implemented');
    }
  }]);

  return dotnetifyVMRouter;
}();

exports.default = dotnetifyVMRouter;

/***/ }),
/* 7 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


Object.defineProperty(exports, "__esModule", {
  value: true
});

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


var _dotnetifyVmRouter = __webpack_require__(6);

var _dotnetifyVmRouter2 = _interopRequireDefault(_dotnetifyVmRouter);

var _jquery = __webpack_require__(3);

var _jquery2 = _interopRequireDefault(_jquery);

var _utils = __webpack_require__(1);

var _utils2 = _interopRequireDefault(_utils);

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

var templateWrapper = function () {
  function templateWrapper(template) {
    _classCallCheck(this, templateWrapper);

    this.template = template;
  }

  _createClass(templateWrapper, [{
    key: 'Id',
    get: function get() {
      return this.template.Id();
    }
  }, {
    key: 'Root',
    get: function get() {
      return this.template.Root();
    }
  }, {
    key: 'UrlPattern',
    get: function get() {
      return this.template.UrlPattern();
    }
  }, {
    key: 'ViewUrl',
    get: function get() {
      return this.template.ViewUrl();
    },
    set: function set(value) {
      this.template.ViewUrl(value);
    }
  }, {
    key: 'JSModuleUrl',
    get: function get() {
      return this.template.JSModuleUrl();
    },
    set: function set(value) {
      this.template.JSModuleUrl(value);
    }
  }, {
    key: 'Target',
    get: function get() {
      return this.template.Target();
    },
    set: function set(value) {
      this.template.Target(value);
    }
  }]);

  return templateWrapper;
}();

var routingStateWrapper = function () {
  function routingStateWrapper(routingState) {
    _classCallCheck(this, routingStateWrapper);

    this.routingState = routingState;
  }

  _createClass(routingStateWrapper, [{
    key: 'Root',
    get: function get() {
      return this.routingState.Root();
    },
    set: function set(value) {
      this.routingState.Root(value);
    }
  }, {
    key: 'Templates',
    get: function get() {
      var templates = typeof this.routingState.Templates == 'function' ? this.routingState.Templates() : null;
      return templates ? templates.map(function (template) {
        return new templateWrapper(template);
      }) : null;
    }
  }, {
    key: 'Active',
    get: function get() {
      return this.routingState.Active();
    },
    set: function set(value) {
      this.routingState.Active(value);
    }
  }, {
    key: 'Origin',
    get: function get() {
      return this.routingState.Origin();
    },
    set: function set(value) {
      this.routingState.Origin(value);
    }
  }]);

  return routingStateWrapper;
}();

var dotnetifyKoVMRouter = function (_dotnetifyVMRouter) {
  _inherits(dotnetifyKoVMRouter, _dotnetifyVMRouter);

  _createClass(dotnetifyKoVMRouter, [{
    key: 'hasRoutingState',
    get: function get() {
      return this.vm.hasOwnProperty('RoutingState');
    }
  }, {
    key: 'RoutingState',
    get: function get() {
      return new routingStateWrapper(this.vm.RoutingState);
    }
  }, {
    key: 'VMRoot',
    get: function get() {
      return this.vm.$element.attr('data-vm-root');
    }
  }, {
    key: 'VMArg',
    get: function get() {
      return this.vm.$element.attr('data-vm-arg');
    }
  }]);

  function dotnetifyKoVMRouter(iVM, iDotNetifyRouter) {
    _classCallCheck(this, dotnetifyKoVMRouter);

    return _possibleConstructorReturn(this, (dotnetifyKoVMRouter.__proto__ || Object.getPrototypeOf(dotnetifyKoVMRouter)).call(this, iVM, iDotNetifyRouter));
  }

  // Dispatch the active routing state to the server.


  _createClass(dotnetifyKoVMRouter, [{
    key: 'dispatchActiveRoutingState',
    value: function dispatchActiveRoutingState(iPath) {
      this.RoutingState.Active = iPath;
    }
  }, {
    key: 'onRouteEnter',
    value: function onRouteEnter(iPath, iTemplate) {
      if (!iTemplate.ViewUrl) iTemplate.ViewUrl = iTemplate.Id + '.html';
      return true;
    }

    // Loads a view into a target element.
    // Method parameters: TargetSelector, ViewUrl, iJsModuleUrl, iVmArg, iCallbackFn

  }, {
    key: 'loadView',
    value: function loadView(iTargetSelector, iViewUrl, iJsModuleUrl, iVmArg, iCallbackFn) {
      var _this2 = this;

      var vm = this.vm;

      // If no view URL is given, empty the target DOM element.
      if (iViewUrl == null || iViewUrl == '') {
        (0, _jquery2.default)(iTargetSelector).empty();
        return;
      }

      var callbackFn = function callbackFn() {
        // If the view model supports routing, add the root path to the view, to be used
        // to build the absolute route path, and view model argument if provided.
        if (_this2.hasRoutingState && _this2.RoutingState.Root) {
          var vmElems = (0, _jquery2.default)(iTargetSelector).find('[data-vm]').toArray();
          vmElems.forEach(function (element) {
            var root = (0, _jquery2.default)(element).attr('data-vm-root');
            root = root != null ? '/' + _utils2.default.trim(vm.RoutingState.Root()) + '/' + _utils2.default.trim(root) : vm.RoutingState.Root();
            (0, _jquery2.default)(element).attr('data-vm-root', root);

            if (iVmArg != null && !_jquery2.default.isEmptyObject(iVmArg)) {
              // If there's already a data-vm-arg, combine the values.
              // Take care not to override server-side routing arguments.
              var vmArg = (0, _jquery2.default)(element).attr('data-vm-arg');
              vmArg = vmArg != null ? _jquery2.default.extend(iVmArg, _jquery2.default.parseJSON(vmArg.replace(/'/g, '"'))) : iVmArg;

              (0, _jquery2.default)(element).attr('data-vm-arg', JSON.stringify(vmArg));
            }
          });
        }

        // Call the callback function.
        if (typeof iCallbackFn === 'function') iCallbackFn.apply(_this2);
      };

      // Provide the opportunity to override the URL.
      iViewUrl = this.router.overrideUrl(iViewUrl);

      vm.$loadView(iTargetSelector, iViewUrl, iJsModuleUrl, iVmArg, callbackFn);
    }

    // Routes to a path.

  }, {
    key: 'manualRouteTo',
    value: function manualRouteTo(iPath, iTarget, iViewUrl, iJSModuleUrl) {
      var template = {
        Target: function Target() {
          return iTarget;
        },
        ViewUrl: function ViewUrl() {
          return iViewUrl;
        },
        JSModuleUrl: function JSModuleUrl() {
          return iJSModuleUrl;
        }
      };
      this.$router.routeTo(iPath, template, true);
    }
  }]);

  return dotnetifyKoVMRouter;
}(_dotnetifyVmRouter2.default);

exports.default = dotnetifyKoVMRouter;

/***/ }),
/* 8 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
/* WEBPACK VAR INJECTION */(function(global) {

Object.defineProperty(exports, "__esModule", {
  value: true
});
/*
PathJS - Copyright (c) 2011 Mike Trpcic, released under the MIT license.
 */
var window = window || global || {};

var Path = {
  // Need this specific version, because latest version is causing issue.
  version: '0.8.5',
  map: function map(path) {
    if (Path.routes.defined.hasOwnProperty(path)) {
      return Path.routes.defined[path];
    } else {
      return new Path.core.route(path);
    }
  },
  root: function root(path) {
    Path.routes.root = path;
  },
  rescue: function rescue(fn) {
    Path.routes.rescue = fn;
  },
  history: {
    initial: {}, // Empty container for "Initial Popstate" checking variables.
    pushState: function pushState(state, title, path) {
      if (Path.history.supported) {
        if (Path.dispatch(path)) {
          history.pushState(state, title, path);
        }
      } else {
        if (Path.history.fallback) {
          window.location.hash = '#' + path;
        }
      }
    },
    popState: function popState(event) {
      var initialPop = !Path.history.initial.popped && location.href == Path.history.initial.URL;
      Path.history.initial.popped = true;
      if (initialPop) return;
      Path.dispatch(document.location.pathname === '/' ? '' : document.location.pathname);
    },
    listen: function listen(fallback) {
      Path.history.supported = !!(window.history && window.history.pushState);
      Path.history.fallback = fallback;

      if (Path.history.supported) {
        Path.history.initial.popped = 'state' in window.history, Path.history.initial.URL = location.href;
        window.onpopstate = Path.history.popState;
      } else {
        if (Path.history.fallback) {
          for (route in Path.routes.defined) {
            if (route.charAt(0) != '#') {
              Path.routes.defined['#' + route] = Path.routes.defined[route];
              Path.routes.defined['#' + route].path = '#' + route;
            }
          }
          Path.listen();
        }
      }
    }
  },
  match: function match(path, parameterize) {
    var params = {},
        route = null,
        possible_routes,
        slice,
        i,
        j,
        compare,
        result;
    for (route in Path.routes.defined) {
      if (route !== null && route !== undefined) {
        route = Path.routes.defined[route];
        possible_routes = route.partition();
        for (j = 0; j < possible_routes.length; j++) {
          slice = possible_routes[j];
          compare = path;
          if (slice.search(/:/) > 0) {
            for (i = 0; i < slice.split('/').length; i++) {
              if (i < compare.split('/').length && slice.split('/')[i].charAt(0) === ':') {
                params[slice.split('/')[i].replace(/:/, '')] = compare.split('/')[i];
                result = compare.split('/');
                result[i] = slice.split('/')[i];
                compare = result.join('/');
              }
            }
          }
          if (slice === compare) {
            if (parameterize) {
              route.params = params;
            }
            return route;
          }
        }
      }
    }
    return null;
  },
  dispatch: function dispatch(passed_route) {
    var previous_route, matched_route;
    if (Path.routes.current !== passed_route) {
      Path.routes.previous = Path.routes.current;
      Path.routes.current = passed_route;
      matched_route = Path.match(passed_route, true);

      if (Path.routes.previous) {
        previous_route = Path.match(Path.routes.previous);
        if (previous_route !== null && previous_route.do_exit !== null) {
          previous_route.do_exit();
        }
      }

      if (matched_route !== null) {
        matched_route.run();
        return true;
      } else {
        if (Path.routes.rescue !== null) {
          Path.routes.rescue();
        }
      }
    }
  },
  listen: function listen() {
    var fn = function fn() {
      Path.dispatch(location.hash);
    };

    if (location.hash === '') {
      if (Path.routes.root !== null) {
        location.hash = Path.routes.root;
      }
    }

    // The 'document.documentMode' checks below ensure that PathJS fires the right events
    // even in IE "Quirks Mode".
    if ('onhashchange' in window && (!document.documentMode || document.documentMode >= 8)) {
      window.onhashchange = fn;
    } else {
      setInterval(fn, 50);
    }

    if (location.hash !== '') {
      Path.dispatch(location.hash);
    }
  },
  core: {
    route: function route(path) {
      this.path = path;
      this.action = null;
      this.do_enter = [];
      this.do_exit = null;
      this.params = {};
      Path.routes.defined[path] = this;
    }
  },
  routes: {
    current: null,
    root: null,
    rescue: null,
    previous: null,
    defined: {}
  }
};

Path.core.route.prototype = {
  to: function to(fn) {
    this.action = fn;
    return this;
  },
  enter: function enter(fns) {
    if (fns instanceof Array) {
      this.do_enter = this.do_enter.concat(fns);
    } else {
      this.do_enter.push(fns);
    }
    return this;
  },
  exit: function exit(fn) {
    this.do_exit = fn;
    return this;
  },
  partition: function partition() {
    var parts = [],
        options = [],
        re = /\(([^}]+?)\)/g,
        text,
        i;
    while (text = re.exec(this.path)) {
      parts.push(text[1]);
    }
    options.push(this.path.split('(')[0]);
    for (i = 0; i < parts.length; i++) {
      options.push(options[options.length - 1] + parts[i]);
    }
    return options;
  },
  run: function run() {
    var halt_execution = false,
        i,
        result,
        previous;

    if (Path.routes.defined[this.path].hasOwnProperty('do_enter')) {
      if (Path.routes.defined[this.path].do_enter.length > 0) {
        for (i = 0; i < Path.routes.defined[this.path].do_enter.length; i++) {
          result = Path.routes.defined[this.path].do_enter[i].apply(this, null);
          if (result === false) {
            halt_execution = true;
            break;
          }
        }
      }
    }
    if (!halt_execution) {
      Path.routes.defined[this.path].action();
    }
  }
};

exports.default = Path;
/* WEBPACK VAR INJECTION */}.call(this, __webpack_require__(0)))

/***/ }),
/* 9 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
/* WEBPACK VAR INJECTION */(function(global) {

Object.defineProperty(exports, "__esModule", {
  value: true
});

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


var _path = __webpack_require__(8);

var _path2 = _interopRequireDefault(_path);

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

var window = window || global || {};

var dotnetifyRouter = function () {
  function dotnetifyRouter(debug) {
    _classCallCheck(this, dotnetifyRouter);

    this.version = '2.0.1';
    this.urlPath = document.location.pathname;

    this.debug = debug;
  }

  // Initialize routing using PathJS.


  // URL path that will be parsed when performing routing.


  _createClass(dotnetifyRouter, [{
    key: 'init',
    value: function init() {
      if (typeof _path2.default !== 'undefined') {
        _path2.default.history.listen(true);
        _path2.default.routes.rescue = function () {
          //window.location.replace(document.location.pathname);
        };
      } else throw new Error('Pathjs library is required for routing.');
    }

    // Map a route to an action.

  }, {
    key: 'mapTo',
    value: function mapTo(iPath, iFn) {
      iPath = iPath.length > 0 ? iPath : '/';
      if (typeof _path2.default !== 'undefined') _path2.default.map(iPath).to(function () {
        iFn(this.params);
      });
    }

    // Match a URL path to a route and run the action.

  }, {
    key: 'match',
    value: function match(iUrlPath) {
      if (typeof _path2.default !== 'undefined') {
        var matched = _path2.default.match(iUrlPath, true);
        if (matched != null) {
          matched.run();
          return true;
        }
      }
      return false;
    }

    // Optional callback to override a URL before performing routing.

  }, {
    key: 'overrideUrl',
    value: function overrideUrl(iUrl) {
      return iUrl;
    }

    // Push state to HTML history.

  }, {
    key: 'pushState',
    value: function pushState(iState, iTitle, iPath) {
      this.urlPath = '';
      if (typeof _path2.default !== 'undefined') _path2.default.history.pushState(iState, iTitle, iPath);
    }

    // Redirect to the a URL.

  }, {
    key: 'redirect',
    value: function redirect(iUrl, viewModels) {
      // Check first whether existing views can handle routing this URL.
      // Otherwise, do a hard browser redirect.
      this.urlPath = iUrl;
      for (var i = 0; i < viewModels.length; i++) {
        var vm = viewModels[i];
        if (vm.$router.routeUrl(iUrl)) {
          if (this.debug) console.log('router> redirected');
          return;
        }
      }
      window.location.replace(iUrl);
    }

    // Called by dotNetify when a view model is ready.

  }, {
    key: '$ready',
    value: function $ready() {
      this.$router.initRouting();
    }

    // Called by dotNetify when a view model receives update.

  }, {
    key: '$update',
    value: function $update(vmData) {
      if (vmData && vmData.RoutingState) this.$router.initRouting();
    }
  }]);

  return dotnetifyRouter;
}();

exports.default = dotnetifyRouter;
/* WEBPACK VAR INJECTION */}.call(this, __webpack_require__(0)))

/***/ }),
/* 10 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


var _dotnetifyKo = __webpack_require__(5);

var _dotnetifyKo2 = _interopRequireDefault(_dotnetifyKo);

var _dotnetifyRouter = __webpack_require__(9);

var _dotnetifyRouter2 = _interopRequireDefault(_dotnetifyRouter);

var _dotnetifyKo3 = __webpack_require__(7);

var _dotnetifyKo4 = _interopRequireDefault(_dotnetifyKo3);

var _jquery = __webpack_require__(3);

var _jquery2 = _interopRequireDefault(_jquery);

var _knockout = __webpack_require__(2);

var ko = _interopRequireWildcard(_knockout);

function _interopRequireWildcard(obj) { if (obj && obj.__esModule) { return obj; } else { var newObj = {}; if (obj != null) { for (var key in obj) { if (Object.prototype.hasOwnProperty.call(obj, key)) newObj[key] = obj[key]; } } newObj.default = obj; return newObj; } }

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

// Add plugin functions.
_dotnetifyKo2.default.ko.router = new _dotnetifyRouter2.default(_dotnetifyKo2.default.debug);

// Inject a view model with functions.
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
_dotnetifyKo2.default.ko.router.$inject = function (iVM) {
  var router = new _dotnetifyKo4.default(iVM, _dotnetifyKo2.default.ko.router);

  // Put functions inside $router namespace.
  iVM.$router = router;
};

// Custom knockout binding to do routing.
ko.bindingHandlers.vmRoute = {
  update: function update(element, valueAccessor, allBindings, viewModel, bindingContext) {
    var vm = bindingContext.$root;
    var route = ko.unwrap(valueAccessor());

    if (!route.hasOwnProperty('Path') || !route.hasOwnProperty('TemplateId')) throw new Error('invalid vmRoute data at ' + element.outerHTML);

    // Build the absolute root path.
    vm.$router.initRoot();

    // If the route path is not defined, use the URL pattern from the associated template.
    // This is so that we don't send the same data twice if both are equal.
    var path = route.Path();
    var template = null;
    if (vm.hasOwnProperty('RoutingState') && typeof vm.RoutingState.Templates === 'function' && vm.RoutingState.Templates() != null) {
      var match = _jquery2.default.grep(vm.RoutingState.Templates(), function (iTemplate) {
        return iTemplate.Id() == route.TemplateId();
      });
      if (match.length > 0) {
        template = match[0];
        if (path == null) {
          path = template.UrlPattern();
          route.Path(path);
        }
      } else if (route.RedirectRoot() == null) throw new Error("vmRoute cannot find route template '" + route.TemplateId() + "' at " + element.outerHTML);
    }

    // If the path has a redirect root, the path doesn't belong to the current root and needs to be
    // redirected to a different one.  Set the absolute path to the HREF attribute, and prevent the
    // default behavior of the anchor click event and instead do push to HTML5 history state, which
    // would attempt to resolve the path first before resorting to hard browser redirect.
    if (route.RedirectRoot() != null) {
      // Combine the redirect root with the view model's root.
      var redirectRoot = route.RedirectRoot();
      if (redirectRoot.charAt(0) == '/') redirectRoot = redirectRoot.substr(0, redirectRoot.length - 1);
      var redirectRootPath = route.RedirectRoot().split('/');

      var url = '';
      var absRoot = vm.$element.attr('data-vm-root');
      if (absRoot != null) {
        var absRootPath = absRoot.split('/');
        for (i = 0; i < absRootPath.length; i++) {
          if (absRootPath[i] != '' && absRootPath[i] == redirectRootPath[0]) break;
          url += absRootPath[i] + '/';
        }
      }
      url += redirectRoot + '/' + path;

      (0, _jquery2.default)(element).attr('href', url).attr('data-vm-route', path).click(function (iEvent) {
        iEvent.preventDefault();
        _dotnetifyKo2.default.ko.router.pushState({}, '', (0, _jquery2.default)(this).attr('href'));
      });
      return;
    }

    // For quick lookup, save the mapping between the path to the route inside the view model.
    if (template == null) throw new Error('vmRoute cannot find any route template at ' + element.outerHTML);

    route.$template = template;
    vm.$router.pathToRoute = vm.$router.pathToRoute || {};
    vm.$router.pathToRoute[path] = route;

    // Set the absolute path to the HREF attribute, and prevent the default behavior of
    // the anchor click event and instead do push to HTML5 history state.
    (0, _jquery2.default)(element).attr('href', vm.$router.toUrl(path)).attr('data-vm-route', path).click(function (iEvent) {
      iEvent.preventDefault();
      _dotnetifyKo2.default.ko.router.pushState({}, '', (0, _jquery2.default)(this).attr('href'));
    });
  }
};

// Register the plugin to dotNetify.
_dotnetifyKo2.default.ko.plugins['router'] = _dotnetifyKo2.default.ko.router;

/***/ }),
/* 11 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
/* WEBPACK VAR INJECTION */(function(module) {var __WEBPACK_AMD_DEFINE_FACTORY__, __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;

var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) { return typeof obj; } : function (obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; };

/// Knockout Mapping plugin v2.4.0
/// (c) 2013 Steven Sanderson, Roy Jacobs - http://knockoutjs.com/
/// License: MIT (http://www.opensource.org/licenses/mit-license.php)
(function (d) {
  "function" === "function" && "object" === ( false ? undefined : _typeof(exports)) && "object" === ( false ? undefined : _typeof(module)) ? d(__webpack_require__(2), exports) :  true ? !(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(2), exports], __WEBPACK_AMD_DEFINE_FACTORY__ = (d),
				__WEBPACK_AMD_DEFINE_RESULT__ = (typeof __WEBPACK_AMD_DEFINE_FACTORY__ === 'function' ?
				(__WEBPACK_AMD_DEFINE_FACTORY__.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__)) : __WEBPACK_AMD_DEFINE_FACTORY__),
				__WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__)) : undefined;
})(function (d, f) {
  function y(b, c) {
    var a, e;for (e in c) {
      if (c.hasOwnProperty(e) && c[e]) if (a = f.getType(b[e]), e && b[e] && "array" !== a && "string" !== a) y(b[e], c[e]);else if ("array" === f.getType(b[e]) && "array" === f.getType(c[e])) {
        a = b;for (var d = e, l = b[e], n = c[e], t = {}, g = l.length - 1; 0 <= g; --g) {
          t[l[g]] = l[g];
        }for (g = n.length - 1; 0 <= g; --g) {
          t[n[g]] = n[g];
        }l = [];n = void 0;for (n in t) {
          l.push(t[n]);
        }a[d] = l;
      } else b[e] = c[e];
    }
  }function E(b, c) {
    var a = {};y(a, b);y(a, c);return a;
  }function z(b, c) {
    for (var a = E({}, b), d = L.length - 1; 0 <= d; d--) {
      var f = L[d];a[f] && (a[""] instanceof Object || (a[""] = {}), a[""][f] = a[f], delete a[f]);
    }c && (a.ignore = h(c.ignore, a.ignore), a.include = h(c.include, a.include), a.copy = h(c.copy, a.copy), a.observe = h(c.observe, a.observe));a.ignore = h(a.ignore, j.ignore);a.include = h(a.include, j.include);a.copy = h(a.copy, j.copy);a.observe = h(a.observe, j.observe);a.mappedProperties = a.mappedProperties || {};a.copiedProperties = a.copiedProperties || {};return a;
  }function h(b, c) {
    "array" !== f.getType(b) && (b = "undefined" === f.getType(b) ? [] : [b]);"array" !== f.getType(c) && (c = "undefined" === f.getType(c) ? [] : [c]);return d.utils.arrayGetDistinctValues(b.concat(c));
  }function F(b, c, a, e, k, l, n) {
    var t = "array" === f.getType(d.utils.unwrapObservable(c));l = l || "";if (f.isMapped(b)) {
      var g = d.utils.unwrapObservable(b)[p];a = E(g, a);
    }var j = n || k,
        h = function h() {
      return a[e] && a[e].create instanceof Function;
    },
        x = function x(b) {
      var f = G,
          g = d.dependentObservable;d.dependentObservable = function (a, b, c) {
        c = c || {};a && "object" == (typeof a === "undefined" ? "undefined" : _typeof(a)) && (c = a);var e = c.deferEvaluation,
            M = !1;c.deferEvaluation = !0;a = new H(a, b, c);if (!e) {
          var g = a,
              e = d.dependentObservable;d.dependentObservable = H;a = d.isWriteableObservable(g);d.dependentObservable = e;a = H({ read: function read() {
              M || (d.utils.arrayRemoveItem(f, g), M = !0);return g.apply(g, arguments);
            }, write: a && function (a) {
              return g(a);
            }, deferEvaluation: !0 });f.push(a);
        }return a;
      };d.dependentObservable.fn = H.fn;d.computed = d.dependentObservable;b = d.utils.unwrapObservable(k) instanceof Array ? a[e].create({ data: b || c, parent: j, skip: N }) : a[e].create({ data: b || c, parent: j });d.dependentObservable = g;d.computed = d.dependentObservable;return b;
    },
        u = function u() {
      return a[e] && a[e].update instanceof Function;
    },
        v = function v(b, f) {
      var g = { data: f || c, parent: j, target: d.utils.unwrapObservable(b) };d.isWriteableObservable(b) && (g.observable = b);return a[e].update(g);
    };if (n = I.get(c)) return n;e = e || "";if (t) {
      var t = [],
          s = !1,
          m = function m(a) {
        return a;
      };
      a[e] && a[e].key && (m = a[e].key, s = !0);d.isObservable(b) || (b = d.observableArray([]), b.mappedRemove = function (a) {
        var c = "function" == typeof a ? a : function (b) {
          return b === m(a);
        };return b.remove(function (a) {
          return c(m(a));
        });
      }, b.mappedRemoveAll = function (a) {
        var c = C(a, m);return b.remove(function (a) {
          return -1 != d.utils.arrayIndexOf(c, m(a));
        });
      }, b.mappedDestroy = function (a) {
        var c = "function" == typeof a ? a : function (b) {
          return b === m(a);
        };return b.destroy(function (a) {
          return c(m(a));
        });
      }, b.mappedDestroyAll = function (a) {
        var c = C(a, m);return b.destroy(function (a) {
          return -1 != d.utils.arrayIndexOf(c, m(a));
        });
      }, b.mappedIndexOf = function (a) {
        var c = C(b(), m);a = m(a);return d.utils.arrayIndexOf(c, a);
      }, b.mappedCreate = function (a) {
        if (-1 !== b.mappedIndexOf(a)) throw Error("There already is an object with the key that you specified.");var c = h() ? x(a) : a;u() && (a = v(c, a), d.isWriteableObservable(c) ? c(a) : c = a);b.push(c);return c;
      });n = C(d.utils.unwrapObservable(b), m).sort();g = C(c, m);s && g.sort();s = d.utils.compareArrays(n, g);n = {};var J,
          A = d.utils.unwrapObservable(c),
          y = {},
          z = !0,
          g = 0;for (J = A.length; g < J; g++) {
        var r = m(A[g]);if (void 0 === r || r instanceof Object) {
          z = !1;break;
        }y[r] = A[g];
      }var A = [],
          B = 0,
          g = 0;for (J = s.length; g < J; g++) {
        var r = s[g],
            q,
            w = l + "[" + g + "]";switch (r.status) {case "added":
            var D = z ? y[r.value] : K(d.utils.unwrapObservable(c), r.value, m);q = F(void 0, D, a, e, b, w, k);h() || (q = d.utils.unwrapObservable(q));w = O(d.utils.unwrapObservable(c), D, n);q === N ? B++ : A[w - B] = q;n[w] = !0;break;case "retained":
            D = z ? y[r.value] : K(d.utils.unwrapObservable(c), r.value, m);q = K(b, r.value, m);F(q, D, a, e, b, w, k);w = O(d.utils.unwrapObservable(c), D, n);A[w] = q;n[w] = !0;break;case "deleted":
            q = K(b, r.value, m);}t.push({ event: r.status, item: q });
      }b(A);a[e] && a[e].arrayChanged && d.utils.arrayForEach(t, function (b) {
        a[e].arrayChanged(b.event, b.item);
      });
    } else if (P(c)) {
      b = d.utils.unwrapObservable(b);if (!b) {
        if (h()) return s = x(), u() && (s = v(s)), s;if (u()) return v(s);b = {};
      }u() && (b = v(b));I.save(c, b);if (u()) return b;Q(c, function (e) {
        var f = l.length ? l + "." + e : e;if (-1 == d.utils.arrayIndexOf(a.ignore, f)) if (-1 != d.utils.arrayIndexOf(a.copy, f)) b[e] = c[e];else if ("object" != _typeof(c[e]) && "array" != typeof c[e] && 0 < a.observe.length && -1 == d.utils.arrayIndexOf(a.observe, f)) b[e] = c[e], a.copiedProperties[f] = !0;else {
          var g = I.get(c[e]),
              k = F(b[e], c[e], a, e, b, f, b),
              g = g || k;if (0 < a.observe.length && -1 == d.utils.arrayIndexOf(a.observe, f)) b[e] = g(), a.copiedProperties[f] = !0;else {
            if (d.isWriteableObservable(b[e])) b[e](d.utils.unwrapObservable(g));else g = void 0 === b[e] ? g : d.utils.unwrapObservable(g), b[e] = g;a.mappedProperties[f] = !0;
          }
        }
      });
    } else switch (f.getType(c)) {case "function":
        u() ? d.isWriteableObservable(c) ? (c(v(c)), b = c) : b = v(c) : b = c;break;default:
        if (d.isWriteableObservable(b)) return q = u() ? v(b) : d.utils.unwrapObservable(c), b(q), q;h() || u();b = h() ? x() : d.observable(d.utils.unwrapObservable(c));u() && b(v(b));}return b;
  }function O(b, c, a) {
    for (var e = 0, d = b.length; e < d; e++) {
      if (!0 !== a[e] && b[e] === c) return e;
    }return null;
  }function R(b, c) {
    var a;c && (a = c(b));"undefined" === f.getType(a) && (a = b);return d.utils.unwrapObservable(a);
  }function K(b, c, a) {
    b = d.utils.unwrapObservable(b);for (var e = 0, f = b.length; e < f; e++) {
      var l = b[e];if (R(l, a) === c) return l;
    }throw Error("When calling ko.update*, the key '" + c + "' was not found!");
  }function C(b, c) {
    return d.utils.arrayMap(d.utils.unwrapObservable(b), function (a) {
      return c ? R(a, c) : a;
    });
  }function Q(b, c) {
    if ("array" === f.getType(b)) for (var a = 0; a < b.length; a++) {
      c(a);
    } else for (a in b) {
      c(a);
    }
  }function P(b) {
    var c = f.getType(b);return ("object" === c || "array" === c) && null !== b;
  }function T() {
    var b = [],
        c = [];this.save = function (a, e) {
      var f = d.utils.arrayIndexOf(b, a);0 <= f ? c[f] = e : (b.push(a), c.push(e));
    };this.get = function (a) {
      a = d.utils.arrayIndexOf(b, a);return 0 <= a ? c[a] : void 0;
    };
  }function S() {
    var b = {},
        c = function c(a) {
      var c;try {
        c = a;
      } catch (d) {
        c = "$$$";
      }a = b[c];void 0 === a && (a = new T(), b[c] = a);return a;
    };this.save = function (a, b) {
      c(a).save(a, b);
    };this.get = function (a) {
      return c(a).get(a);
    };
  }var p = "__ko_mapping__",
      H = d.dependentObservable,
      B = 0,
      G,
      I,
      L = ["create", "update", "key", "arrayChanged"],
      N = {},
      x = { include: ["_destroy"], ignore: [], copy: [], observe: [] },
      j = x;f.isMapped = function (b) {
    return (b = d.utils.unwrapObservable(b)) && b[p];
  };f.fromJS = function (b) {
    if (0 == arguments.length) throw Error("When calling ko.fromJS, pass the object you want to convert.");
    try {
      B++ || (G = [], I = new S());var c, a;2 == arguments.length && (arguments[1][p] ? a = arguments[1] : c = arguments[1]);3 == arguments.length && (c = arguments[1], a = arguments[2]);a && (c = E(c, a[p]));c = z(c);var e = F(a, b, c);a && (e = a);if (! --B) for (; G.length;) {
        var d = G.pop();d && d();
      }e[p] = E(e[p], c);return e;
    } catch (f) {
      throw B = 0, f;
    }
  };f.fromJSON = function (b) {
    var c = d.utils.parseJson(b);arguments[0] = c;return f.fromJS.apply(this, arguments);
  };f.updateFromJS = function () {
    throw Error("ko.mapping.updateFromJS, use ko.mapping.fromJS instead. Please note that the order of parameters is different!");
  };f.updateFromJSON = function () {
    throw Error("ko.mapping.updateFromJSON, use ko.mapping.fromJSON instead. Please note that the order of parameters is different!");
  };f.toJS = function (b, c) {
    j || f.resetDefaultOptions();if (0 == arguments.length) throw Error("When calling ko.mapping.toJS, pass the object you want to convert.");if ("array" !== f.getType(j.ignore)) throw Error("ko.mapping.defaultOptions().ignore should be an array.");if ("array" !== f.getType(j.include)) throw Error("ko.mapping.defaultOptions().include should be an array.");
    if ("array" !== f.getType(j.copy)) throw Error("ko.mapping.defaultOptions().copy should be an array.");c = z(c, b[p]);return f.visitModel(b, function (a) {
      return d.utils.unwrapObservable(a);
    }, c);
  };f.toJSON = function (b, c) {
    var a = f.toJS(b, c);return d.utils.stringifyJson(a);
  };f.defaultOptions = function () {
    if (0 < arguments.length) j = arguments[0];else return j;
  };f.resetDefaultOptions = function () {
    j = { include: x.include.slice(0), ignore: x.ignore.slice(0), copy: x.copy.slice(0) };
  };f.getType = function (b) {
    if (b && "object" === (typeof b === "undefined" ? "undefined" : _typeof(b))) {
      if (b.constructor === Date) return "date";if (b.constructor === Array) return "array";
    }return typeof b === "undefined" ? "undefined" : _typeof(b);
  };f.visitModel = function (b, c, a) {
    a = a || {};a.visitedObjects = a.visitedObjects || new S();var e,
        k = d.utils.unwrapObservable(b);if (P(k)) a = z(a, k[p]), c(b, a.parentName), e = "array" === f.getType(k) ? [] : {};else return c(b, a.parentName);a.visitedObjects.save(b, e);var l = a.parentName;Q(k, function (b) {
      if (!(a.ignore && -1 != d.utils.arrayIndexOf(a.ignore, b))) {
        var j = k[b],
            g = a,
            h = l || "";"array" === f.getType(k) ? l && (h += "[" + b + "]") : (l && (h += "."), h += b);g.parentName = h;
        if (!(-1 === d.utils.arrayIndexOf(a.copy, b) && -1 === d.utils.arrayIndexOf(a.include, b) && k[p] && k[p].mappedProperties && !k[p].mappedProperties[b] && k[p].copiedProperties && !k[p].copiedProperties[b] && "array" !== f.getType(k))) switch (f.getType(d.utils.unwrapObservable(j))) {case "object":case "array":case "undefined":
            g = a.visitedObjects.get(j);e[b] = "undefined" !== f.getType(g) ? g : f.visitModel(j, c, a);break;default:
            e[b] = c(j, a.parentName);}
      }
    });return e;
  };
});
/* WEBPACK VAR INJECTION */}.call(this, __webpack_require__(4)(module)))

/***/ }),
/* 12 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
var __WEBPACK_AMD_DEFINE_FACTORY__, __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;

/*! jQuery UI - v1.11.4 - 2015-10-01
* http://jqueryui.com
* Includes: widget.js
* Copyright 2015 jQuery Foundation and other contributors; Licensed MIT */

(function (e) {
   true ? !(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(3)], __WEBPACK_AMD_DEFINE_FACTORY__ = (e),
				__WEBPACK_AMD_DEFINE_RESULT__ = (typeof __WEBPACK_AMD_DEFINE_FACTORY__ === 'function' ?
				(__WEBPACK_AMD_DEFINE_FACTORY__.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__)) : __WEBPACK_AMD_DEFINE_FACTORY__),
				__WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__)) : undefined;
})(function (e) {
  var t = 0,
      i = Array.prototype.slice;e.cleanData = function (t) {
    return function (i) {
      var s, n, a;for (a = 0; null != (n = i[a]); a++) {
        try {
          s = e._data(n, "events"), s && s.remove && e(n).triggerHandler("remove");
        } catch (o) {}
      }t(i);
    };
  }(e.cleanData), e.widget = function (t, i, s) {
    var n,
        a,
        o,
        r,
        h = {},
        l = t.split(".")[0];return t = t.split(".")[1], n = l + "-" + t, s || (s = i, i = e.Widget), e.expr[":"][n.toLowerCase()] = function (t) {
      return !!e.data(t, n);
    }, e[l] = e[l] || {}, a = e[l][t], o = e[l][t] = function (e, t) {
      return this._createWidget ? (arguments.length && this._createWidget(e, t), void 0) : new o(e, t);
    }, e.extend(o, a, { version: s.version, _proto: e.extend({}, s), _childConstructors: [] }), r = new i(), r.options = e.widget.extend({}, r.options), e.each(s, function (t, s) {
      return e.isFunction(s) ? (h[t] = function () {
        var e = function e() {
          return i.prototype[t].apply(this, arguments);
        },
            n = function n(e) {
          return i.prototype[t].apply(this, e);
        };return function () {
          var t,
              i = this._super,
              a = this._superApply;return this._super = e, this._superApply = n, t = s.apply(this, arguments), this._super = i, this._superApply = a, t;
        };
      }(), void 0) : (h[t] = s, void 0);
    }), o.prototype = e.widget.extend(r, { widgetEventPrefix: a ? r.widgetEventPrefix || t : t }, h, { constructor: o, namespace: l, widgetName: t, widgetFullName: n }), a ? (e.each(a._childConstructors, function (t, i) {
      var s = i.prototype;e.widget(s.namespace + "." + s.widgetName, o, i._proto);
    }), delete a._childConstructors) : i._childConstructors.push(o), e.widget.bridge(t, o), o;
  }, e.widget.extend = function (t) {
    for (var s, n, a = i.call(arguments, 1), o = 0, r = a.length; r > o; o++) {
      for (s in a[o]) {
        n = a[o][s], a[o].hasOwnProperty(s) && void 0 !== n && (t[s] = e.isPlainObject(n) ? e.isPlainObject(t[s]) ? e.widget.extend({}, t[s], n) : e.widget.extend({}, n) : n);
      }
    }return t;
  }, e.widget.bridge = function (t, s) {
    var n = s.prototype.widgetFullName || t;e.fn[t] = function (a) {
      var o = "string" == typeof a,
          r = i.call(arguments, 1),
          h = this;return o ? this.each(function () {
        var i,
            s = e.data(this, n);return "instance" === a ? (h = s, !1) : s ? e.isFunction(s[a]) && "_" !== a.charAt(0) ? (i = s[a].apply(s, r), i !== s && void 0 !== i ? (h = i && i.jquery ? h.pushStack(i.get()) : i, !1) : void 0) : e.error("no such method '" + a + "' for " + t + " widget instance") : e.error("cannot call methods on " + t + " prior to initialization; " + "attempted to call method '" + a + "'");
      }) : (r.length && (a = e.widget.extend.apply(null, [a].concat(r))), this.each(function () {
        var t = e.data(this, n);t ? (t.option(a || {}), t._init && t._init()) : e.data(this, n, new s(a, this));
      })), h;
    };
  }, e.Widget = function () {}, e.Widget._childConstructors = [], e.Widget.prototype = { widgetName: "widget", widgetEventPrefix: "", defaultElement: "<div>", options: { disabled: !1, create: null }, _createWidget: function _createWidget(i, s) {
      s = e(s || this.defaultElement || this)[0], this.element = e(s), this.uuid = t++, this.eventNamespace = "." + this.widgetName + this.uuid, this.bindings = e(), this.hoverable = e(), this.focusable = e(), s !== this && (e.data(s, this.widgetFullName, this), this._on(!0, this.element, { remove: function remove(e) {
          e.target === s && this.destroy();
        } }), this.document = e(s.style ? s.ownerDocument : s.document || s), this.window = e(this.document[0].defaultView || this.document[0].parentWindow)), this.options = e.widget.extend({}, this.options, this._getCreateOptions(), i), this._create(), this._trigger("create", null, this._getCreateEventData()), this._init();
    }, _getCreateOptions: e.noop, _getCreateEventData: e.noop, _create: e.noop, _init: e.noop, destroy: function destroy() {
      this._destroy(), this.element.unbind(this.eventNamespace).removeData(this.widgetFullName).removeData(e.camelCase(this.widgetFullName)), this.widget().unbind(this.eventNamespace).removeAttr("aria-disabled").removeClass(this.widgetFullName + "-disabled " + "ui-state-disabled"), this.bindings.unbind(this.eventNamespace), this.hoverable.removeClass("ui-state-hover"), this.focusable.removeClass("ui-state-focus");
    }, _destroy: e.noop, widget: function widget() {
      return this.element;
    }, option: function option(t, i) {
      var s,
          n,
          a,
          o = t;if (0 === arguments.length) return e.widget.extend({}, this.options);if ("string" == typeof t) if (o = {}, s = t.split("."), t = s.shift(), s.length) {
        for (n = o[t] = e.widget.extend({}, this.options[t]), a = 0; s.length - 1 > a; a++) {
          n[s[a]] = n[s[a]] || {}, n = n[s[a]];
        }if (t = s.pop(), 1 === arguments.length) return void 0 === n[t] ? null : n[t];n[t] = i;
      } else {
        if (1 === arguments.length) return void 0 === this.options[t] ? null : this.options[t];o[t] = i;
      }return this._setOptions(o), this;
    }, _setOptions: function _setOptions(e) {
      var t;for (t in e) {
        this._setOption(t, e[t]);
      }return this;
    }, _setOption: function _setOption(e, t) {
      return this.options[e] = t, "disabled" === e && (this.widget().toggleClass(this.widgetFullName + "-disabled", !!t), t && (this.hoverable.removeClass("ui-state-hover"), this.focusable.removeClass("ui-state-focus"))), this;
    }, enable: function enable() {
      return this._setOptions({ disabled: !1 });
    }, disable: function disable() {
      return this._setOptions({ disabled: !0 });
    }, _on: function _on(t, i, s) {
      var n,
          a = this;"boolean" != typeof t && (s = i, i = t, t = !1), s ? (i = n = e(i), this.bindings = this.bindings.add(i)) : (s = i, i = this.element, n = this.widget()), e.each(s, function (s, o) {
        function r() {
          return t || a.options.disabled !== !0 && !e(this).hasClass("ui-state-disabled") ? ("string" == typeof o ? a[o] : o).apply(a, arguments) : void 0;
        }"string" != typeof o && (r.guid = o.guid = o.guid || r.guid || e.guid++);var h = s.match(/^([\w:-]*)\s*(.*)$/),
            l = h[1] + a.eventNamespace,
            u = h[2];u ? n.delegate(u, l, r) : i.bind(l, r);
      });
    }, _off: function _off(t, i) {
      i = (i || "").split(" ").join(this.eventNamespace + " ") + this.eventNamespace, t.unbind(i).undelegate(i), this.bindings = e(this.bindings.not(t).get()), this.focusable = e(this.focusable.not(t).get()), this.hoverable = e(this.hoverable.not(t).get());
    }, _delay: function _delay(e, t) {
      function i() {
        return ("string" == typeof e ? s[e] : e).apply(s, arguments);
      }var s = this;return setTimeout(i, t || 0);
    }, _hoverable: function _hoverable(t) {
      this.hoverable = this.hoverable.add(t), this._on(t, { mouseenter: function mouseenter(t) {
          e(t.currentTarget).addClass("ui-state-hover");
        }, mouseleave: function mouseleave(t) {
          e(t.currentTarget).removeClass("ui-state-hover");
        } });
    }, _focusable: function _focusable(t) {
      this.focusable = this.focusable.add(t), this._on(t, { focusin: function focusin(t) {
          e(t.currentTarget).addClass("ui-state-focus");
        }, focusout: function focusout(t) {
          e(t.currentTarget).removeClass("ui-state-focus");
        } });
    }, _trigger: function _trigger(t, i, s) {
      var n,
          a,
          o = this.options[t];if (s = s || {}, i = e.Event(i), i.type = (t === this.widgetEventPrefix ? t : this.widgetEventPrefix + t).toLowerCase(), i.target = this.element[0], a = i.originalEvent) for (n in a) {
        n in i || (i[n] = a[n]);
      }return this.element.trigger(i, s), !(e.isFunction(o) && o.apply(this.element[0], [i].concat(s)) === !1 || i.isDefaultPrevented());
    } }, e.each({ show: "fadeIn", hide: "fadeOut" }, function (t, i) {
    e.Widget.prototype["_" + t] = function (s, n, a) {
      "string" == typeof n && (n = { effect: n });var o,
          r = n ? n === !0 || "number" == typeof n ? i : n.effect || i : t;n = n || {}, "number" == typeof n && (n = { duration: n }), o = !e.isEmptyObject(n), n.complete = a, n.delay && s.delay(n.delay), o && e.effects && e.effects.effect[r] ? s[t](n) : r !== t && s[r] ? s[r](n.duration, n.easing, a) : s.queue(function (i) {
        e(this)[t](), a && a.call(s[0]), i();
      });
    };
  }), e.widget;
});

/***/ }),
/* 13 */
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


var _utils = __webpack_require__(1);

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
/* 14 */
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


var _utils = __webpack_require__(1);

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
/* 15 */
/***/ (function(module, exports) {

module.exports = __WEBPACK_EXTERNAL_MODULE__15__;

/***/ }),
/* 16 */
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
/* 17 */
/***/ (function(module, exports, __webpack_require__) {

var jQuery = module.exports = __webpack_require__(16),
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
/* 18 */
/***/ (function(module, exports, __webpack_require__) {


/*!
* jquery-deferred
* Copyright(c) 2011 Hidden <zzdhidden@gmail.com>
* MIT Licensed
*/

/**
* Library version.
*/

var jQuery = module.exports = __webpack_require__(17),
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
/* 19 */
/***/ (function(module, exports, __webpack_require__) {


module.exports = __webpack_require__(18);

/***/ }),
/* 20 */
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

var jQueryDeferred = __webpack_require__(19);
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
/* WEBPACK VAR INJECTION */}.call(this, __webpack_require__(0), __webpack_require__(4)(module)))

/***/ }),
/* 21 */
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


var _utils = __webpack_require__(1);

var _jqueryShim = __webpack_require__(20);

var _jqueryShim2 = _interopRequireDefault(_jqueryShim);

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

var signalRNetCore = __webpack_require__(15);
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
/* 22 */
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


var _dotnetifyHub = __webpack_require__(21);

var _dotnetifyHub2 = _interopRequireDefault(_dotnetifyHub);

var _dotnetifyHubLocal = __webpack_require__(14);

var _dotnetifyHubLocal2 = _interopRequireDefault(_dotnetifyHubLocal);

var _dotnetifyHubWebapi = __webpack_require__(13);

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

/***/ }),
/* 23 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


var _dotnetifyKo = __webpack_require__(5);

var _dotnetifyKo2 = _interopRequireDefault(_dotnetifyKo);

__webpack_require__(10);

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

module.exports = _dotnetifyKo2.default;

/***/ })
/******/ ]);
});
//# sourceMappingURL=dotnetify-ko.js.map
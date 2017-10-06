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

var jQueryDeferred = require("jquery-deferred");
var jQueryShim = jQueryDeferred.extend(
   function (selector) {

      if (selector === window || selector.document)
         return {
            0: selector,
            on: function (iEvent, iHandler) { window.addEventListener(iEvent, iHandler); },
            bind: function (iEvent, iHandler) { window.addEventListener(iEvent, iHandler, false); },
            unbind: function (iEvent, iHandler) { window.removeEventListener(iEvent, iHandler, false); }
         };

      if (typeof selector !== "string")
         selector.events = selector.events || {};

      return {
         0: selector,

         bind: function (iEvent, iHandler) {
            var event = selector.events[iEvent] || [];
            event.push(iHandler);
            selector.events[iEvent] = event;
         },

         unbind: function (iEvent, iHandler) {
            var handlers = selector.events[iEvent] || [];
            if (iHandler) {
               var idx = handlers.indexOf(iHandler);
               if (idx !== -1)
                  handlers.splice(idx, 1);
            } else
               handlers = [];
            selector.events[iEvent] = handlers;
         },

         triggerHandler: function (iEvent, iArgs) {
            var handlers = selector.events[iEvent] || [];
            var args = [{ type: iEvent }];
            if (Array.isArray(iArgs))
               iArgs.forEach(function (arg) { args.push(arg); });
            else if (iArgs)
               args.push(iArgs);
            handlers.forEach(function (handler) { handler.apply(this, args); });
         },

         load: function (iUrl, iArgs, iHandler) {
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
            return { abort: function (reason) { return request.abort(reason); } };
         }
      };
   },
   jQueryDeferred,
   {
      support: { cors: true },

      trim: function (iStr) { return typeof iStr === "string" ? iStr.trim() : iStr; },

      inArray: function (iArray, iItem) { return iArray.indexOf(iItem) !== -1; },

      makeArray: function makeArray(iArray) { return [].slice.call(iArray, 0); },

      merge: function (iArray1, iArray2) { Array.prototype.push.apply(iArray1, iArray2); return iArray1; },

      isEmptyObject: function (iObj) { return !iObj || Object.keys(iObj).length === 0 },

      ajax: function (iOptions) {
         var request = new window.XMLHttpRequest();
         request.onreadystatechange = function () {
            if (request.readyState !== 4)
               return;
            if (request.status === 200 && !request._hasError) {
               try {
                  iOptions.success && iOptions.success(JSON.parse(request.responseText));
               }
               catch (error) {
                  iOptions.success && iOptions.success(request.responseText);
               }
            }
            else
               iOptions.error && iOptions.error(request);
         };
         request.open(iOptions.type, iOptions.url);
         request.setRequestHeader("content-type", iOptions.contentType);
         request.send(iOptions.data.data && "data=" + iOptions.data.data);
         return {
            abort: function (reason) { return request.abort(reason); }
         };
      },

      getScript: function (iUrl, iSuccess) {
         var done = false;
         var promise = jQueryDeferred.Deferred();
         var head = document.getElementsByTagName("head")[0];
         var script = document.createElement("script");
         script.src = iUrl;
         script.onload = script.onreadystatechange = function () {
            if (!done && (!this.readyState || this.readyState == "loaded" || this.readyState == "complete")) {
               done = true;
               script.onload = script.onreadystatechange = null;
               head.removeChild(script);
               if (typeof iSuccess === "function")
                  iSuccess();
               promise.resolve();
            }
         };
         head.appendChild(script);
         return promise;
      }
   }
);

if (typeof window !== "undefined")
   window.jQuery = window.jQuery || jQueryShim;

if (typeof exports === "object" && typeof module === "object")
   module.exports = jQueryShim;
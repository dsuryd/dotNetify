/* 
Copyright 2016 Dicky Suryadi

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

// Support using RequireJS that loads our app.js, or being placed in <script> tag.
(function (factory) {
   if (typeof define === "function" && define["amd"]) {
      define(['jquery', 'knockout', 'dotnetify'], factory);
   }
   else {
      factory(jQuery, ko, binder);
   }
}
(function ($, ko, dotnetify) {

   // Add extension for binding to Google polymer elements.
   dotnetify.binder.extensions["polymer-paper"] =
      {
         version: "1.0.5",

         // Called by dotNetify before binding is applied.
         setBindings: function (elem, bind) {
            var vm = this;
            var id = elem.id;

            if (!vm.$binder.bindable(id))
               return bind;

            var tagName = elem.tagName.toLowerCase();
            if (tagName == "paper-input") {
               bind += bind.length > 0 ? ", " : "";
               var type = elem.type;
               if (type == "search")
                  bind += "textInput: " + id;
               else
                  bind += "value: " + id;
               vm[id].$bound = true;
            }

            return bind;
         }
      }
}))
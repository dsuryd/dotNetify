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
import Vue from 'vue';

// Call a method when a property value changes.
Vue.directive('vmOn', {
  bind: function(el, binding, vnode) {
    // Parse the value, which should be in object literal { property: fnName }.
    const match = /{(.*):(.*)}/.exec(binding.expression);
    if (match != null) {
      const propName = match[1].trim();
      const methodName = match[2].trim();
      const vue = vnode.context;

      if (!vue.hasOwnProperty(propName)) throw new Error(`v-vmOn property '${propName}' is not defined`);

      if (!vue.hasOwnProperty(methodName) && typeof vue[methodName] == 'function')
        throw new Error(`v-vmOn method '${propName}' is not defined or not a function`);

      vue.$watch(propName, () => vue[methodName](el));
    }
  }
});

// Route link directive for anchor tags.
Vue.directive('vmRoute', {
  bind: function(el, binding, vnode) {
    const vue = vnode.context;
    const route = binding.value;
    el.href = route && vue.vm ? vue.vm.$route(route) : '';
    el.addEventListener('click', function(e) {
      e.preventDefault();
      vue.vm.$handleRoute(e);
    });
  },
  componentUpdated: function(el, binding, vnode) {
    const vue = vnode.context;
    const route = binding.value;
    if (route && vue.vm) el.href = vue.vm.$route(route);
  }
});

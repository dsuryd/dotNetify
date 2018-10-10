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
import dotnetify from './dotnetify-vue';

export default {
  name: 'Scope',
  props: {
    vm: String,
    tag: String
  },
  inject: { scoped: { name: 'scoped', default: null } },
  provide: function() {
    const _this = this;
    return {
      scoped: vmId => _this.getScope(vmId),
      connect: (vmId, component, options) => {
        vmId = _this.getScope(vmId);
        return dotnetify.vue.connect(vmId, component, options);
      }
    };
  },
  render: function(createElement) {
    return createElement(this.tag || 'div', null, this.$slots.default);
  },
  methods: {
    getScope: function(vmId) {
      let scope = this.scoped ? this.scoped(this.vm) : this.vm;
      return scope ? scope + '.' + vmId : vmId;
    }
  }
};

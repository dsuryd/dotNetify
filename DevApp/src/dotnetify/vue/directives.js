import Vue from 'vue';

// Call a method when a property value changes.
Vue.directive('vmOn', {
  bind: function(el, binding, vnode) {
    // Parse the value, which should be in object literal { property: fnName }.
    const match = /{(.*):(.*)}/.exec(binding.expression);
    if (match != null) {
      const propName = match[1].trim();
      const methodName = match[2].trim();
      const vm = vnode.context;

      if (!vm.hasOwnProperty(propName)) throw new Error(`v-vmOn property '${propName}' is not defined`);

      if (!vm.hasOwnProperty(methodName) && typeof vm[methodName] == 'function')
        throw new Error(`v-vmOn method '${propName}' is not defined or not a function`);

      vm.$watch(propName, () => vm[methodName](el));
    }
  }
});

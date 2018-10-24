<template>
  <div>
    <nav class="navbar navbar-expand-lg navbar-light bg-light">
      <a
        v-for="link in Links"
        :key="link.Id"
        v-vmRoute="link.Route"
        style="padding-right: 2rem"
      >{{link.Caption}}</a>
    </nav>
    <div id="Content"/>
  </div>
</template>

<script>
import dotnetify from 'dotnetify/vue';

export default {
  created() {
    this.vm = dotnetify.vue.connect('App', this);
    this.vm.onRouteEnter = (path, template) => (template.Target = 'Content');
  },
  destroyed() {
    this.vm.$destroy();
  },
  data() { return { Links: [] } }
}
</script>

<style lang="scss">
.navbar {
  justify-content: left;
}
</style>

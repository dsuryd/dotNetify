<script lang="ts">
import dotnetify from "dotnetify/vue";

export default {
  name: "HelloWorld",
  created: function () {
    this.vm = dotnetify.vue.connect("HelloWorld", this);
  },
  unmounted: function () {
    this.vm.$destroy();
  },
  data: function () {
    return { Greetings: "", ServerTime: "" };
  }
};
</script>

<template>
  <section>
    <h1>{{ Greetings }}</h1>

    <p>
      <a href="https://dotnetify.net/" target="_blank">DotNetify Docs</a>
      |
      <a href="https://vitejs.dev/guide/features.html" target="_blank"> Vite Docs </a>
      |
      <a href="https://v3.vuejs.org/" target="_blank">Vue 3 Docs</a>
    </p>

    <div>
      <label>Server time is: {{ ServerTime }}</label>
    </div>

    <br />
    <p>
      Edit
      <code>components/HelloWorld.vue</code> to test hot module replacement.
    </p>
  </section>
</template>

<style scoped>
a {
  color: #42b983;
}

label {
  margin: 0 0.5em;
  font-weight: bold;
}

code {
  background-color: #eee;
  padding: 2px 4px;
  border-radius: 4px;
  color: #304455;
}
</style>

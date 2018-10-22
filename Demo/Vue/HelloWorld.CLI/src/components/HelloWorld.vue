<template>
  <div class="hello">
    <h1>{{ msg }}</h1>
    <div>
      <h3>{{ state.Greetings }}</h3>
      <p>Server time is: {{ state.ServerTime }}</p>
    </div>
  </div>
</template>

<script>
import dotnetify from 'dotnetify/vue';
export default dotnetify.vue.component({ name: "hello-world", props: { msg: String } }, "HelloWorld")
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h3 { font-weight: 500; }
</style>

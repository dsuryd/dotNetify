<template>
  <div>
    <p>{{Greetings}}</p>
    <p>Server time is: {{ServerTime}}</p>
  </div>
</template>

<script>
import dotnetify from 'dotnetify/vue';

export default {
  name: 'HelloWorld',
  created: function () {
    this.vm = dotnetify.vue.connect("HelloWorld", this);
  },
  destroyed: function () {
    this.vm.$destroy();
  },
  data: function () {
    return { Greetings: '', ServerTime: '' }
  }
}
</script>
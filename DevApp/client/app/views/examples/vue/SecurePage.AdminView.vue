<template>
  <div v-if="TokenIssuer">
    <h5>Admin-only view:</h5>
    <div>{{TokenIssuer}}</div>
    <div>{{TokenValidFrom}}</div>
    <div>{{TokenValidTo}}</div>
  </div>
</template>

<script>
import dotnetify from 'dotnetify/vue';

export default {
  name: 'AdminView',
  props: {
    accessToken: String
  },
  created() {
    let authHeader = { Authorization: 'Bearer ' + this.accessToken };
    this.vm = dotnetify.vue.connect("AdminSecurePageVM", this, {
      headers: authHeader,
      exceptionHandler: ex => { }
    });
  },
  destroyed() {
    this.vm.$destroy();
  },
  data() {
    return {
      TokenIssuer: null,
      TokenValidFrom: '',
      TokenValidTo: '',
      ExceptionType: '',
      Message: ''
    }
  }
}
</script>
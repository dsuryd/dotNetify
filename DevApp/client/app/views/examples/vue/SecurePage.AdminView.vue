<template>
  <section v-if="TokenIssuer">
    <h5>Admin-only view:</h5>
    <div>{{ TokenIssuer }}</div>
    <div>{{ TokenValidFrom }}</div>
    <div>{{ TokenValidTo }}</div>
    <br />
    <button class="btn btn-secondary" @click="refreshToken">Refresh Token</button>
  </section>
</template>

<script>
import dotnetify from "dotnetify/vue";
import { fetchToken } from "./SecurePage.vue";

export default {
  name: "AdminView",
  props: {
    accessToken: String
  },
  created() {
    let authHeader = { Authorization: "Bearer " + this.accessToken };
    this.vm = dotnetify.vue.connect("AdminSecurePageVM", this, {
      headers: authHeader,
      exceptionHandler: ex => {}
    });
  },
  unmounted() {
    this.vm.$destroy();
  },
  data() {
    return {
      TokenIssuer: null,
      TokenValidFrom: "",
      TokenValidTo: "",
      ExceptionType: "",
      Message: ""
    };
  },
  methods: {
    refreshToken: function () {
      fetchToken("admin", "dotnetify").then(token =>
        this.vm.$dispatch({
          $headers: { Authorization: "Bearer " + token.access_token },
          Refresh: true
        })
      );
    }
  }
};
</script>

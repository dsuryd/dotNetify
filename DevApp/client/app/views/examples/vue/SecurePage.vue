<template>
  <div>
    <div v-if="accessToken" class="card logout">
      <button class="btn btn-primary" @click="signOut">Sign Out</button>
    </div>
    <div v-if="!accessToken" class="card">
      <div class="card-header">
        <h4>Sign in</h4>
      </div>
      <div class="card-body">
        <div>
          <label>User name:</label>
          <input
            type="text"
            class="form-control"
            placeholder="Type guest or admin"
            v-model.lazy="username"
          >
          <b>{{loginError}}</b>
        </div>
        <div>
          <label>Password:</label>
          <input type="password" class="form-control" v-model.lazy="password">
          <b>{{loginError}}</b>
        </div>
      </div>
      <div class="card-footer">
        <button class="btn btn-primary" @click="submit">Submit</button>
      </div>
    </div>
    <div v-if="!accessToken" class="card">
      <div class="card-body">
        <h4>Not authenticated</h4>
      </div>
    </div>
    <secure-view v-if="accessToken" :accessToken="accessToken" @expiredAccess="signOut"/>
  </div>
</template>

<script>
import SecureView from './SecurePage.SecureView.vue'

export default {
  name: "SecurePage",
  components: {
    'secure-view': SecureView
  },
  data() {
    return {
      username: '',
      password: 'dotnetify',
      loginError: null,
      accessToken: window.sessionStorage.getItem('access_token')
    }
  },
  methods: {
    submit: function () {
      this.signIn(this.username, this.password);
    },

    signIn(username, password) {
      fetch('/token', {
        method: 'post',
        mode: 'no-cors',
        body: 'username=' + username + '&password=' + password + '&grant_type=password&client_id=dotnetifydemo',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded;charset=UTF-8' }
      })
        .then(response => {
          if (!response.ok) throw Error(response.statusText);
          return response.json();
        })
        .then(token => {
          window.sessionStorage.setItem('access_token', token.access_token);
          this.loginError = null;
          this.accessToken = token.access_token;
        })
        .catch(error => this.loginError = 'Invalid user name or password');
    },
    signOut() {
      window.sessionStorage.clear();
      this.accessToken = null;
    }
  }
}
</script>
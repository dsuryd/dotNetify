##### SecurePage.vue

```html
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
          <input type="text" class="form-control" placeholder="Type guest or admin" v-model.lazy="username">
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
  data: function () {
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
```

##### SecurePage.SecureView.vue

```html
<template>
  <div class="card">
    <div class="card-header">
      <h4>{{SecureCaption}}</h4>
    </div>
    <div class="card-body">
      <b>{{SecureData}}</b>
      <admin-view :accessToken="accessToken" @expiredAccess="emitExpiredAccess"/>
    </div>
  </div>
</template>

<script>
import dotnetify from 'dotnetify/vue';
import AdminView from './SecurePage.AdminView.vue';

export default {
  name: 'SecureView',
  components: {
    'admin-view': AdminView
  },
  props: {
    accessToken: String
  },
  created: function () {
    let authHeader = { Authorization: 'Bearer ' + this.accessToken };
    this.vm = dotnetify.vue.connect("SecurePageVM", this, {
      headers: authHeader,
      exceptionHandler: ex => this.onException(ex)
    });
  },
  destroyed: function () {
    this.vm.$destroy();
  },
  data: function () {
    return {
      SecureCaption: 'Not authenticated',
      SecureData: '',
      ExceptionType: '',
      Message: ''
    }
  },
  methods: {
    onException: function (exception) {
      if (exception.name == 'UnauthorizedAccessException') this.emitExpiredAccess();
    },
    emitExpiredAccess: function () {
      this.$emit('expiredAccess');
    }
  }
}
</script>
```

##### SecurePage.AdminView.vue

```html
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
  created: function () {
    let authHeader = { Authorization: 'Bearer ' + this.accessToken };
    this.vm = dotnetify.vue.connect("AdminSecurePageVM", this, {
      headers: authHeader,
      exceptionHandler: ex => {}
    });
  },
  destroyed: function () {
    this.vm.$destroy();
  },
  data: function () {
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
```
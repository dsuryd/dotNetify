<template>
  <div class="card">
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
          v-model.lazy="userName"
        >
        <b>{{errorText}}</b>
      </div>
      <div>
        <label>Password:</label>
        <input type="password" class="form-control" v-model.lazy="password">
        <b>{{errorText}}</b>
      </div>
    </div>
    <div class="card-footer">
      <button class="btn btn-primary" @click="submit">Submit</button>
    </div>
  </div>
</template>

<script>
export default {
  name: "SecurePage",
  data: function () {
    return {
      userName: '',
      password: 'dotnetify',
      errorText: ''
    }
  },
  methods: {
    submit: function () {

    }
  }
}
</script>
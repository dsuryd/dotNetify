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
  created() {
    let authHeader = { Authorization: 'Bearer ' + this.accessToken };
    this.vm = dotnetify.vue.connect("SecurePageVM", this, {
      headers: authHeader,
      exceptionHandler: ex => this.onException(ex)
    });
  },
  destroyed() {
    this.vm.$destroy();
  },
  data() {
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
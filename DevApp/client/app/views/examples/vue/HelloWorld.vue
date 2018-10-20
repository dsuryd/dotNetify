<template>
  <div>
    <section>
      <div>
        <label>First name:</label>
        <input type="text" class="form-control" v-model.lazy="FirstName">
      </div>
      <div>
        <label>Last name:</label>
        <input type="text" class="form-control" v-model.lazy="LastName">
      </div>
    </section>
    <div>
      Full name is
      <b>{{FullName}}</b>
    </div>
  </div>
</template>

<script>
import dotnetify from 'dotnetify/vue';

export default {
  name: 'HelloWorld',
  created() {
    this.vm = dotnetify.vue.connect("HelloWorldVM", this, { watch: ['FirstName', 'LastName'] });
  },
  destroyed() {
    this.vm.$destroy();
  },
  data() {
    return {
      FirstName: '',
      LastName: '',
      FullName: ''
    }
  }
}
</script>
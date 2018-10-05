<template>
  <div id="HelloWorld">
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
      <b>
        <span>{{FullName}}</span>
      </b>
    </div>
  </div>
</template>

<script>
import dotnetify from 'dotnetify/vue';

export default {
  name: 'HelloWorld',
  created: function () {
    this.vm = dotnetify.vue.connect("HelloWorldVM", this, { watch: ['FirstName', 'LastName'] });
  },
  data() {
    return { FirstName: '', LastName: '', FullName: '' }
  }
}
</script>
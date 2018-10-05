<template>
<div id="HelloWorld">
  <section>
    <div>
      <label>First name:</label>
      <input type="text" class="form-control" />
    </div>
    <div>
      <label>Last name:</label>
      <input type="text" class="form-control"  />
    </div>
  </section>
  <div>
    Full name is
    <b>
      <span />
    </b>
  </div>
</div>
</template>

<script>
export default {
  name: 'helloworld',
  data () {
    return {
      FirstName: '',
      LastName: '',
      FullName: ''
    }
  }
}
</script>
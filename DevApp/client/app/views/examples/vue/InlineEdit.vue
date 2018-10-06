<template>
  <div>
    <span v-if="!edit" class="editable" v-on:click="toggleEdit">{{value}}</span>
    <input
      v-else
      type="text"
      class="form-control"
      ref="inputElem"
      v-model.lazy="input"
      v-on:blur="onBlur"
    >
  </div>
</template>

<script>
export default {
  name: 'InlineEdit',
  props: ['value'],
  data: function () {
    return {
      edit: false,
      input: this.value
    }
  },
  methods: {
    toggleEdit() {
      this.edit = !this.edit;
      if (this.edit)
        setTimeout(_ => this.$refs.inputElem.focus());

    },
    onBlur: function (e) {
      this.toggleEdit();

    }
  }
}
</script>
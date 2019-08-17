<template>
  <div>
    <span v-if="!edit" class="editable" @click="toggleEdit">{{value}}</span>
    <input
      v-else
      type="text"
      class="form-control"
      ref="inputElem"
      v-model.lazy="input"
      @blur="update"
      @keydown.enter="update"
    />
  </div>
</template>

<script>
export default {
  name: 'InlineEdit',
  props: {
    value: String
  },
  data() {
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
    update: function () {
      this.toggleEdit();
      this.$emit('update', this.input);
    }
  },
  watch: {
    value: function (newVal, oldVal) {
      this.input = newVal;
    }
  }
}
</script>
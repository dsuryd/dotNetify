<template>
  <div id="BookDetails" class="modal fade">
    <div class="modal-dialog modal-dialog-centered">
      <div class="modal-content">
        <div class="modal-body" style="display: flex">
          <img class="thumbnail" data-bind="attr: {src: Book.ImageUrl}">
          <div style="margin-left: 1rem">
            <h3>{{Book.Title}}</h3>
            <h5>{{Book.Author}}</h5>
            <button class="btn btn-primary">Buy</button>
          </div>
        </div>
        <div class="modal-footer">
          <button id="Back" type="button" class="btn btn-success" data-dismiss="modal">Back</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import dotnetify from 'dotnetify/vue';

export default {
  name: 'Book',
  created: function () {
    this.vm = dotnetify.vue.connect("BookDetailsVM", this);
  },
  destroyed: function () {
    this.vm.$destroy();
  },
  data: function () {
    return {
      Book: {},
      RoutingState: { Active: '' }
    }
  }
}
</script>

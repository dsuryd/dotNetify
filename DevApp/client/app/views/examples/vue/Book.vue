<template>
  <div id="BookDetails" class="modal fade">
    <div class="modal-dialog modal-dialog-centered">
      <div class="modal-content">
        <div class="modal-body" style="display: flex" v-if="Book">
          <img class="thumbnail" :src="Book.ImageUrl" />
          <div style="margin-left: 1rem">
            <h3>{{Book.Title}}</h3>
            <h5>{{Book.Author}}</h5>
            <button class="btn btn-primary">Buy</button>
          </div>
        </div>
        <div class="modal-body" v-if="Book === null">
          Sorry, we couldn't find any book title that matches
          <b>{{SearchTitle}}</b>.
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
  created() {
    this.vm = dotnetify.vue.connect(
      'BookDetailsVM',
      this
    );
  },
  mounted() {
    const self = this;
    $('#BookDetails').modal();
    $('#BookDetails').on('hidden.bs.modal', function(e) {
      self.vm.$routeTo(self.BookDefaultRoute);
      self.$destroy();
    });
  },
  destroyed() {
    $('#BookDetails').modal('dispose');
    $('.modal-backdrop').remove();
    this.vm.$destroy();
  },
  data() {
    return {
      Book: {},
      SearchTitle: '',
      BookDefaultRoute: null
    };
  }
};
</script>

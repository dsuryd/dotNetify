##### BookStore.vue

```html
<template>
  <section>
    <div>
      <div v-for="book in Books" :key="book.Info.Id">
        <center>
          <a v-vmRoute="book.Route">
            <img class="thumbnail" :src="book.Info.ImageUrl">
          </a>
          <div>
            <b>{{book.Info.Title}}</b>
            <div>
              by
              <span>{{book.Info.Author}}</span>
            </div>
          </div>
        </center>
      </div>
    </div>
    <div id="BookPanel"/>
  </section>
</template>

<script>
import dotnetify from 'dotnetify/vue';

export default {
  name: 'BookStore',
  created() {
    this.vm = dotnetify.vue.connect("BookStoreVM", this);
    this.vm.onRouteEnter = (path, template) => (template.Target = 'BookPanel');
  },
  destroyed() {
    this.vm.$destroy();
  },
  data() {
    return {
      Books: []
    }
  }
}
</script>
```

##### Book.vue

```html
<template>
  <div id="BookDetails" class="modal fade">
    <div class="modal-dialog modal-dialog-centered">
      <div class="modal-content">
        <div class="modal-body" style="display: flex" v-if="Book">
          <img class="thumbnail" :src="Book.ImageUrl">
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
  created() {
    this.vm = dotnetify.vue.connect("BookDetailsVM", this);
  },
  mounted() {
    const self = this;
    $('#BookDetails').modal();
    $('#BookDetails').on('hidden.bs.modal', function (e) {
      self.$destroy();
      window.history.back();
    });
  },
  destroyed() {
    this.vm.$destroy();
  },
  data() {
    return {
      Book: {}
    }
  }
}
</script>
```
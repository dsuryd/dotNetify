<template>
  <section>
    <header>
      Each product here is represented by a unique URL that can be entered into the address bar to go directly to that
      specific product page.
    </header>
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
<template>
  <!-- <Scope vm="CompositeViewVM"> -->
  <div>
    <section>
      <MovieTable/>
    </section>
    <aside>
      <MovieDetails/>
      <MovieFilter/>
    </aside>
  </div>
  <!-- </Scope> -->
</template>

<script>
import dotnetify from 'dotnetify/vue';
import MovieTable from './CompositeView.MovieTable.vue';
import MovieDetails from './CompositeView.MovieDetails.vue';
import MovieFilter from './CompositeView.MovieFilter.vue';

export default {
  name: 'CompositeView',
  components: {
    'Scope': dotnetify.vue.Scope,
    'MovieTable': MovieTable,
    'MovieDetails': MovieDetails,
    'MovieFilter': MovieFilter
  },
  created: function () {
    this.vm = dotnetify.vue.connect("CompositeViewVM", this);
  },
  destroyed: function () {
    this.vm.$destroy();
  },
  data: function () {
    return {
    }
  }
}
</script>
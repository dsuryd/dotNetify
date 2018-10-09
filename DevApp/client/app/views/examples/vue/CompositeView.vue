<template>
  <Scope vm="CompositeViewVM">
    <Scope tag="section" vm="FilterableMovieTableVM">
      <MovieTable/>
    </Scope>
    <aside>
      <MovieDetails/>
      <MovieFilter/>
    </aside>
  </Scope>
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
  }
}
</script>
<template>
  <div class="card">
    <header class="card-header">
      <b>{{Movie.Movie}}</b>
      <div>{{Movie.Year}}</div>
    </header>
    <section class="card-body">
      <b>Director</b>
      <p>{{Movie.Director}}</p>
      <b>Cast</b>
      <div>
        <div v-for="(cast, i) in Movie.Cast.split(',')" :key="i">{{cast}}</div>
      </div>
    </section>
  </div>
</template>

<script>
import dotnetify from 'dotnetify/vue';

export default {
  name: 'MovieDetails',
  created: function () {
    this.vm = dotnetify.vue.connect("CompositeViewVM.MovieDetailsVM", this);
  },
  destroyed: function () {
    this.vm.$destroy();
  },
  data: function () {
    return {
      Movie: { Cast: '' }
    }
  }
}
</script>
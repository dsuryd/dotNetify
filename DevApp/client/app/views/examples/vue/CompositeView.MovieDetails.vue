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
export default {
  name: 'MovieDetails',
  inject: ['connect'],
  created() {
    this.vm = this.connect("MovieDetailsVM", this);
  },
  destroyed() {
    this.vm.$destroy();
  },
  data() {
    return {
      Movie: { Cast: '' }
    }
  }
}
</script>
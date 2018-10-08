<template>
  <div>
    <table>
      <thead>
        <tr>
          <th v-for="(header, i) in Headers" :key="i">{{header}}</th>
        </tr>
      </thead>
      <tbody>
        <tr
          v-for="data in Data"
          :key="data.Rank"
          :class="{selected: SelectedKey == data.Rank}"
          @click="SelectedKey = data.Rank"
        >
          <td>{{data.Rank}}</td>
          <td>{{data.Movie}}</td>
          <td>{{data.Year}}</td>
          <td>{{data.Director}}</td>
        </tr>
      </tbody>
    </table>
    <div class="pagination">
      <div
        v-for="page in Pagination"
        :key="page"
        :class="{current: SelectedPage === page}"
        @click="SelectedPage = page"
      >{{page}}</div>
    </div>
  </div>
</template>

<script>
import dotnetify from 'dotnetify/vue';

export default {
  name: 'MovieTable',
  created: function () {
    this.vm = dotnetify.vue.connect("CompositeViewVM.FilterableMovieTableVM.MovieTableVM", this, { watch: ['SelectedKey', 'SelectedPage'] });
  },
  destroyed: function () {
    this.vm.$destroy();
  },
  data: function () {
    return {
      Headers: [],
      Data: [],
      Pagination: [],
      SelectedKey: 0,
      SelectedPage: 0
    }
  }
}
</script>
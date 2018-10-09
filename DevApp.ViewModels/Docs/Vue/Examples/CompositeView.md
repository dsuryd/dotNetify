##### CompositeView.vue

```html
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
```

##### MovieTable.vue

```html
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
export default {
  name: 'MovieTable',
  inject: ['connect'],
  created: function () {
    this.vm = this.connect("MovieTableVM", this, { watch: ['SelectedKey', 'SelectedPage'] });
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
```

##### MovieDetails.vue

```html
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
  created: function () {
    this.vm = this.connect("MovieDetailsVM", this);
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
```

##### MovieFilter.vue

```html
<template>
  <form v-on:submit.prevent>
    <div class="filter card">
      <div class="card-header">Filters</div>
      <div class="card-body">
        <select
          class="form-control"
          :value="filter"
          @change="updateFilterDropdown($event.target.value)"
        >
          <option v-for="(text, i) in movieProps" :key="i" :value="text">{{text}}</option>
        </select>
        <div class="operation">
          <div class="form-check" v-for="(op, i) in operations" :key="i">
            <input
              class="form-check-input"
              type="radio"
              :id="op"
              :value="op"
              :checked="operation === op"
              @change="operation = op"
            >
            <label class="form-check-label" :for="op">{{op}}</label>
          </div>
        </div>
        <input class="form-control" v-model="text">
        <div>
          <div class="chip" v-for="filter in filters" :key="filter.id">
            {{filter.property}} {{filter.operation}} {{filter.text}}
            <div @click="handleDelete(filter.id)">
              <i class="material-icons">clear</i>
            </div>
          </div>
        </div>
      </div>
      <div class="card-footer">
        <button class="btn btn-primary" @click="handleApply" :disabled="!text">Apply</button>
      </div>
    </div>
  </form>
</template>

<script>
export default {
  name: 'MovieFilter',
  inject: ['connect'],
  created: function () {
    this.vm = this.connect("FilterableMovieTableVM.MovieFilterVM", this);
  },
  destroyed: function () {
    this.vm.$destroy();
  },
  data: function () {
    return {
      filters: [],
      filterId: 0,
      filter: 'Any',
      movieProps: ['Any', 'Rank', 'Movie', 'Year', 'Cast', 'Director'],
      operation: 'contains',
      operations: ['contains'],
      text: ''
    }
  },
  methods: {
    handleApply: function () {
      let newId = this.filterId + 1;
      let filter = {
        id: newId,
        property: this.filter,
        operation: this.operation,
        text: this.text
      };

      this.filterId = newId;
      this.filters = [filter, ...this.filters];
      this.text = '';

      this.vm.$dispatch({ Apply: filter });
      this.updateFilterDropdown('Any'); // Reset filter dropdown.
    },
    handleDelete: function (id) {
      this.vm.$dispatch({ Delete: id });
      this.filters = this.filters.filter(filter => filter.id != id);
    },
    updateFilterDropdown: function (filter) {
      this.filter = filter;
      if (filter == 'Rank' || filter == 'Year') {
        this.operations = ['equals', '>=', '<='];
        this.operation = 'equals';
      }
      else {
        this.operations = ['contains'];
        this.operation = 'contains';
      }
    }
  }
}
</script>
```
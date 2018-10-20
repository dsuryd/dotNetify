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
  created() {
    this.vm = this.connect("FilterableMovieTableVM.MovieFilterVM", this);
  },
  destroyed() {
    this.vm.$destroy();
  },
  data() {
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
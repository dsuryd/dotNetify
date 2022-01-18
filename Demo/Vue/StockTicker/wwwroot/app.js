const StockTicker = {
  template: `
    <form onsubmit="return false">
      <div class="input-group w-25 m-1 mb-3">
        <input class="form-control" type="text" placeholder="Symbol" v-model="symbol" />
        <button class="btn btn-primary" type="submit" @click="add">Add</button>
      </div>
    </form>
    <section class="d-flex flex-wrap">
      <div class="m-1 w-25" v-for="(price, symbol) in StockPrices" :key="symbol">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">{{ symbol }}</h5>
            <h1 class="card-text"> {{ price }}</h1>
          </div>
        </div>
      </div>
    <section>
`,
  created() {
    this.vm = dotnetify.vue.connect("StockTicker", this);
  },
  unmounted() {
    this.vm.$destroy();
  },
  data() {
    return { symbol: "", StockPrices: [] };
  },
  methods: {
    add() {
      this.vm.$dispatch({ AddSymbol: this.symbol });
      this.symbol = "";
    }
  }
};

Vue.createApp(StockTicker).mount("#app");
const StockTicker = Vue.defineCustomElement({
  template: `
    <form onsubmit="return false">
      <div>
        <input type="text" placeholder="Enter symbol" v-model="symbol" />
        <button type="submit" @click="add">Add</button>
      </div>
    </form>
    <div v-for="(price, symbol) in StockPrices" :key="symbol">
      <h5>{{ symbol }}</h5>
      <h1>{{ price }}</h1>
    </div>
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
});

customElements.define('stock-ticker', StockTicker);
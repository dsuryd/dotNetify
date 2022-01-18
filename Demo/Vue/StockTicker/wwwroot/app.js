const StockTicker = {
  template: `
    <form onsubmit="return false">
      <input type="text" placeholder="Symbol " v-model="symbol" />
      <button type="submit" @click="add">Add</button>
      <ul v-for="(price, symbol) in StockPrices" :key="symbol">
        <li>{{ symbol }}: {{ price }}</li>
      </ul>
    </form>
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
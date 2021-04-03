import { createApp } from "vue";
import StockTicker from "./StockTicker.vue";

document.getElementById("App").innerHTML = "<stock-ticker />";
createApp({}).component("stock-ticker", StockTicker).mount("#App");

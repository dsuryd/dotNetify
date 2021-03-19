import { createApp } from "vue";
import HelloWorld from "./HelloWorld.vue";

document.getElementById("App").innerHTML = "<hello-world />";
createApp({}).component("hello-world", HelloWorld).mount("#App");

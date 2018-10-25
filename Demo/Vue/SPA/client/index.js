import dotnetify from 'dotnetify/vue';
import App from './App.vue';
import SimpleList from './SimpleList.vue';
import LiveChart from './LiveChart.vue';

dotnetify.debug = true;

// Set the components to global window to make it accessible to dotNetify routing.
Object.assign(window, { SimpleList, LiveChart });

dotnetify.vue.router.$mount('#App', App);

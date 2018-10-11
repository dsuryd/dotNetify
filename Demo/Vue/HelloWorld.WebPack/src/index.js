import Vue from 'vue';
import HelloWorld from './HelloWorld.vue';

document.getElementById('App').innerHTML = '<hello-world />';
new Vue({
	el: '#App',
	components: {
		'hello-world': HelloWorld
	}
});

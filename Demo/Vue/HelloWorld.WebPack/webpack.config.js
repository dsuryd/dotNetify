'use strict';

const { VueLoaderPlugin } = require('vue-loader');

module.exports = {
	mode: 'development',
	entry: { main: './src/index.js' },
	output: {
		path: __dirname + '/wwwroot/dist',
		publicPath: '/dist/'
	},
	resolve: {
		modules: [ 'src', 'node_modules' ],
		alias: { vue$: 'vue/dist/vue.esm.js' }
	},
	module: {
		rules: [ { test: /\.vue$/, use: 'vue-loader' } ]
	},
	plugins: [ new VueLoaderPlugin() ]
};

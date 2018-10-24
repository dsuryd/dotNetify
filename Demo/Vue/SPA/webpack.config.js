'use strict';

const { VueLoaderPlugin } = require('vue-loader');

module.exports = {
	mode: 'development',
	entry: { main: './client/index.js' },
	output: {
		path: __dirname + '/wwwroot/dist',
		publicPath: '/dist/'
	},
	resolve: {
		modules: [ 'client', 'node_modules' ],
		alias: { vue$: 'vue/dist/vue.esm.js' }
	},
	module: {
		rules: [ { test: /\.vue$/, use: 'vue-loader' }, { test: /\.scss$/, use: [ 'vue-style-loader', 'css-loader', 'sass-loader' ] } ]
	},
	plugins: [ new VueLoaderPlugin() ]
};

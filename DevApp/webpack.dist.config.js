'use strict';

const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');

const baseExport = {
	module: {
		rules: [
			{ test: /\.jsx?$/, use: 'babel-loader', exclude: /node_modules/ },
			{ test: /\.tsx?$/, use: 'awesome-typescript-loader?silent=true' },
			{ test: /\.css$/, use: [ MiniCssExtractPlugin.loader, 'css-loader?minimize' ] },
			{ test: /\.svg$/, use: 'svg-url-loader?noquotes=true' },
			{ test: /\.(png|jpg|jpeg|gif)$/, use: 'url-loader?limit=25000' },
			{ test: /\.(eot|ttf|woff(2)?)(\?v=\d+\.\d+\.\d+)?/, loader: 'url-loader' }
		]
	},
	resolve: {
		modules: [ 'src', 'node_modules' ],
		extensions: [ '.js', '.jsx', '.tsx' ]
	},
	plugins: [ new MiniCssExtractPlugin(), new CopyWebpackPlugin([ { from: './src/dotnetify/libs/signalR-netfx.js', to: './' } ]) ]
};

module.exports = [
	{
		mode: 'production',
		entry: {
			dotnetify: './src/dotnetify/react/index.js'
		},
		output: {
			path: __dirname + '/dist/dist',
			filename: 'dotnetify-react.js',
			library: 'dotnetify',
			libraryTarget: 'umd'
		},
		externals: {
			react: {
				commonjs: 'react',
				commonjs2: 'react',
				amd: 'react',
				root: 'React'
			},
			'react-dom': {
				commonjs: 'react-dom',
				commonjs2: 'react-dom',
				amd: 'react-dom',
				root: 'ReactDOM'
			},
			'@aspnet/signalr': {
				commonjs: '@aspnet/signalr',
				commonjs2: '@aspnet/signalr',
				amd: '@aspnet/signalr',
				root: 'signalR'
			}
		},
		resolve: baseExport.resolve,
		module: baseExport.module,
		plugins: baseExport.plugins
	},
	{
		mode: 'production',
		entry: {
			dotnetify: './src/dotnetify/knockout/index.js'
		},
		output: {
			path: __dirname + '/dist/dist',
			filename: 'dotnetify-ko.js',
			library: 'dotnetify',
			libraryTarget: 'umd'
		},
		externals: {
			knockout: {
				commonjs: 'knockout',
				commonjs2: 'knockout',
				amd: 'knockout',
				root: 'ko'
			},
			jquery: {
				commonjs: 'jquery',
				commonjs2: 'jquery',
				amd: 'jquery',
				root: 'jQuery'
			},
			'@aspnet/signalr': {
				commonjs: '@aspnet/signalr',
				commonjs2: '@aspnet/signalr',
				amd: '@aspnet/signalr',
				root: 'signalR'
			}
		},
		resolve: baseExport.resolve,
		module: baseExport.module,
		plugins: baseExport.plugins
	}
];

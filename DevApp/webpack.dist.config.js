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
    mode: 'development',
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
      react: 'React',
      'react-dom': 'ReactDOM',
      '@aspnet/signalr': 'signalR'
    },
    resolve: baseExport.resolve,
    module: baseExport.module,
    plugins: baseExport.plugins
  },
  {
    mode: 'development',
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
      knockout: 'knockout',
      jquery: 'jquery',
      '@aspnet/signalr': 'signalR'
    },
    resolve: baseExport.resolve,
    module: baseExport.module,
    plugins: baseExport.plugins
  }
];

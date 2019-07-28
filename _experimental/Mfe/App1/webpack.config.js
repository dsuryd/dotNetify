'use strict';

const path = require('path');
const webpack = require('webpack');
const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin;

module.exports = env => ({
  mode: 'development',
  entry: {
    app: './client/main.js'
  },
  output: {
    filename: '[name].js',
    path: __dirname + '/wwwroot/dist',
    publicPath: '/dist/'
  },
  resolve: {
    modules: [ 'client', 'node_modules' ],
    extensions: [ '.js', '.jsx' ],
    alias: { 'styled-components': path.resolve(__dirname, 'node_modules', 'dotnetify-elements/node_modules/styled-components') }
  },
  module: {
    rules: [
      { test: /\.jsx?$/, use: 'babel-loader', exclude: /node_modules/ },
      { test: /\.css$/, use: [ 'css-loader?minimize' ] },
      { test: /\.(png|jpg|jpeg|gif|svg)$/, use: 'url-loader' }
    ]
  },
  externals: {
    dotnetify: 'dotnetify',
    'dotnetify-elements': 'dotNetifyElements',
    react: 'React',
    'react-dom': 'ReactDOM'
  },
  devtool: 'source-map',
  plugins: [ new webpack.ContextReplacementPlugin(/moment[/\\]locale$/, /en/), env === 'analyze' && new BundleAnalyzerPlugin() ].filter(
    x => x
  )
});

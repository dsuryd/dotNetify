'use strict';

module.exports = {
  mode: 'development',
  entry: { main: './client/index.js' },
  output: {
    path: __dirname + '/wwwroot/dist',
    publicPath: '/dist/'
  },
  resolve: {
    modules: [ 'client', 'node_modules' ]
  },
  module: {
    rules: [ { test: /\.jsx?$/, use: 'babel-loader', exclude: /node_modules/ } ]
  }
};

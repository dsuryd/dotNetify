'use strict';

module.exports = {
  mode: 'development',
  entry: { main: './src/index.js' },
  output: {
    path: __dirname + '/wwwroot/dist',
    publicPath: '/dist/'
  },
  resolve: {
    modules: [ 'src', 'node_modules' ]
  },
  module: {
    rules: [ { test: /\.tsx?$/, use: 'awesome-typescript-loader?silent=true' } ]
  }
};

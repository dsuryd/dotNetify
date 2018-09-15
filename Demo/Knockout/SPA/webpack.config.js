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
    rules: [
      { test: /\.tsx?$/, use: 'awesome-typescript-loader?silent=true' },
      { test: /\.scss$/, use: [ 'style-loader', 'css-loader', 'sass-loader' ] }
    ]
  }
};

'use strict';

module.exports = {
  mode: 'development',
  entry: { main: './src/index.js' },
  output: {
    chunkFilename: '[id].js',
    path: __dirname + '/wwwroot/dist',
    publicPath: '/dist/'
  },
  resolve: {
    modules: [ 'src', 'node_modules' ]
  },
  module: {
    rules: [
      { test: /\.jsx?$/, use: 'babel-loader', exclude: /node_modules/ },
      { test: /\.css$/, use: [ 'style-loader', 'css-loader' ] }
    ]
  }
};

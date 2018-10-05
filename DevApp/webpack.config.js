'use strict';

const { VueLoaderPlugin } = require('vue-loader');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');

module.exports = {
  mode: 'development',
  entry: {
    app: './client/main.js',
    dotnetify: './src/dotnetify/index.js',
    'dotnetify-ko': './src/dotnetify/knockout/index.js'
  },
  output: {
    filename: '[name].js',
    path: __dirname + '/wwwroot/dist',
    publicPath: '/dist/'
  },
  devtool: 'source-map',
  resolve: {
    modules: [ 'src', 'client', 'node_modules' ],
    extensions: [ '.js', '.jsx', '.tsx', '.vue' ],
    alias: { vue$: 'vue/dist/vue.esm.js' }
  },
  module: {
    rules: [
      { test: /\.jsx?$/, use: 'babel-loader', exclude: /node_modules/ },
      { test: /\.tsx?$/, use: 'awesome-typescript-loader?silent=true' },
      { test: /\.vue$/, use: 'vue-loader' },
      { test: /\.html$/, loader: 'html-loader' },
      { test: /\.css$/, use: [ MiniCssExtractPlugin.loader, 'css-loader?minimize' ] },
      { test: /\.svg$/, use: 'svg-url-loader?noquotes=true' },
      { test: /\.(png|jpg|jpeg|gif)$/, use: 'url-loader' },
      { test: /\.(eot|ttf|woff(2)?)(\?v=\d+\.\d+\.\d+)?/, loader: 'url-loader' }
    ]
  },
  externals: {
    dotnetify: 'dotnetify',
    'dotnetify-elements': 'dotNetifyElements',
    knockout: 'ko',
    react: 'React',
    'react-dom': 'ReactDOM'
  },
  plugins: [ new VueLoaderPlugin(), new MiniCssExtractPlugin() ]
};

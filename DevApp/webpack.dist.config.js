'use strict';

const MiniCssExtractPlugin = require('mini-css-extract-plugin');

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
  plugins: [ new MiniCssExtractPlugin() ]
};

module.exports = [
  {
    mode: 'production',
    entry: {
      dotnetify: './src/dotnetify/index.js'
    },
    output: {
      path: __dirname + '/dist',
      filename: '[name].js',
      library: 'dotNetify',
      libraryTarget: 'umd'
    },
    resolve: {
      modules: [ 'src', 'node_modules' ],
      extensions: [ '.js', '.jsx', '.tsx' ]
    },
    externals: [ 'prop-types', 'react', 'react-dom' ],
    module: baseExport.module,
    plugins: baseExport.plugins
  },
  {
    mode: 'production',
    entry: {
      dotnetify: './src/dotnetify/react/index.js'
    },
    output: {
      path: __dirname + '/dist/react',
      filename: 'index.js',
      library: 'dotNetifyReact',
      libraryTarget: 'umd'
    },
    resolve: {
      modules: [ 'src', 'node_modules' ],
      extensions: [ '.js', '.jsx', '.tsx' ]
    },
    externals: [ 'prop-types', 'react', 'react-dom' ],
    module: baseExport.module,
    plugins: baseExport.plugins
  }
];

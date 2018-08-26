'use strict';

const MiniCssExtractPlugin = require('mini-css-extract-plugin');

module.exports = {
   mode: 'development',
   entry: {
      app: './client/main.js',
      dotnetify: './src/dotnetify'
   },
   output: {
      filename: '[name].js',
      path: __dirname + '/wwwroot/dist',
      publicPath: '/dist/'
   },
   resolve: {
      modules: [ 'src', 'client', 'node_modules' ],
      extensions: [ '.js', '.jsx', '.tsx' ]
   },
   module: {
      rules: [
         { test: /\.jsx?$/, use: 'babel-loader', exclude: /node_modules/ },
         { test: /\.tsx?$/, use: 'awesome-typescript-loader?silent=true' },
         { test: /\.css$/, use: [ MiniCssExtractPlugin.loader, 'css-loader?minimize' ] },
         { test: /\.(png|jpg|jpeg|gif|svg)$/, use: 'url-loader?limit=25000' },
         { test: /\.(eot|svg|ttf|woff(2)?)(\?v=\d+\.\d+\.\d+)?/, loader: 'url-loader' }
      ]
   },
   externals: {
      dotnetify: 'dotnetify',
      react: 'React',
      'react-dom': 'ReactDOM'
   },
   plugins: [ new MiniCssExtractPlugin() ]
};

'use strict';

const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const UglifyJsPlugin = require('uglifyjs-webpack-plugin');

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
  plugins: [
    new MiniCssExtractPlugin(),
    new CopyWebpackPlugin([
      { from: './src/dotnetify/libs/signalR-netfx.js', to: './' },
      { from: './src/dotnetify/libs/jquery-shim.js', to: './' },
      { from: './src/dotnetify/typings', to: './typings' }
    ])
  ],
  optimization: {
    minimize: true,
    minimizer: [
      new UglifyJsPlugin({
        include: /\.min\.js$/
      })
    ]
  },
  devtool: 'source-map'
};

module.exports = [
  Object.assign(
    {
      mode: 'production',
      entry: {
        'dotnetify-react': './src/dotnetify/react/index.js',
        'dotnetify-react.min': './src/dotnetify/react/index.js',
        dotnetify: './src/dotnetify/react/dotnetify-react.js'
      },
      output: {
        path: __dirname + '/dist/dist',
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
      }
    },
    baseExport
  ),
  Object.assign(
    {
      mode: 'production',
      entry: {
        'dotnetify-ko': './src/dotnetify/knockout/index.js',
        'dotnetify-ko.min': './src/dotnetify/knockout/index.js'
      },
      output: {
        path: __dirname + '/dist/dist',
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
      }
    },
    baseExport
  ),
  Object.assign(
    {
      mode: 'production',
      entry: {
        'dotnetify-vue': './src/dotnetify/vue/index.js',
        'dotnetify-vue.min': './src/dotnetify/vue/index.js'
      },
      output: {
        path: __dirname + '/dist/dist',
        library: 'dotnetify',
        libraryTarget: 'umd'
      },
      externals: {
        vue: {
          commonjs: 'vue',
          commonjs2: 'vue',
          amd: 'vue',
          root: 'Vue'
        },
        '@aspnet/signalr': {
          commonjs: '@aspnet/signalr',
          commonjs2: '@aspnet/signalr',
          amd: '@aspnet/signalr',
          root: 'signalR'
        }
      }
    },
    baseExport
  )
];

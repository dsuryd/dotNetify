"use strict";

const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CopyWebpackPlugin = require("copy-webpack-plugin");
const UglifyJsPlugin = require("uglifyjs-webpack-plugin");

const baseExport = {
  module: {
    rules: [
      { test: /\.jsx?$/, use: "babel-loader", exclude: /node_modules/ },
      { test: /\.tsx?$/, use: "ts-loader", exclude: /node_modules/ },
      { test: /\.css$/, use: [MiniCssExtractPlugin.loader, "css-loader?minimize"] },
      { test: /\.svg$/, use: "svg-url-loader?noquotes=true" },
      { test: /\.(png|jpg|jpeg|gif)$/, use: "url-loader?limit=25000" },
      { test: /\.(eot|ttf|woff(2)?)(\?v=\d+\.\d+\.\d+)?/, loader: "url-loader" }
    ]
  },
  resolve: {
    modules: ["src", "node_modules"],
    extensions: [".js", ".jsx", ".ts", ".tsx"]
  },
  plugins: [
    new MiniCssExtractPlugin(),
    new CopyWebpackPlugin([
      { from: "./src/dotnetify/libs/signalR-netfx.js", to: "./" },
      { from: "./src/dotnetify/libs/jquery-shim.js", to: "./" },
      { from: "./typings/_typings", to: "./typings" }
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
  devtool: "source-map"
};

module.exports = [
  Object.assign(
    {
      mode: "production",
      entry: {
        "dotnetify-react": "./src/dotnetify/react/index.ts",
        "dotnetify-react.min": "./src/dotnetify/react/index.ts",
        dotnetify: "./src/dotnetify/react/dotnetify-react.ts"
      },
      output: {
        path: __dirname + "/dist/dist",
        library: "dotnetify",
        libraryTarget: "umd"
      },
      externals: {
        react: {
          commonjs: "react",
          commonjs2: "react",
          amd: "react",
          root: "React"
        },
        "react-dom": {
          commonjs: "react-dom",
          commonjs2: "react-dom",
          amd: "react-dom",
          root: "ReactDOM"
        },
        "@microsoft/signalr": {
          commonjs: "@microsoft/signalr",
          commonjs2: "@microsoft/signalr",
          amd: "@microsoft/signalr",
          root: "signalR"
        }
      }
    },
    baseExport
  ),
  Object.assign(
    {
      mode: "production",
      entry: {
        "dotnetify-ko": "./src/dotnetify/knockout/index.js",
        "dotnetify-ko.min": "./src/dotnetify/knockout/index.js"
      },
      output: {
        path: __dirname + "/dist/dist",
        library: "dotnetify",
        libraryTarget: "umd"
      },
      externals: {
        knockout: {
          commonjs: "knockout",
          commonjs2: "knockout",
          amd: "knockout",
          root: "ko"
        },
        jquery: {
          commonjs: "jquery",
          commonjs2: "jquery",
          amd: "jquery",
          root: "jQuery"
        },
        "@microsoft/signalr": {
          commonjs: "@microsoft/signalr",
          commonjs2: "@microsoft/signalr",
          amd: "@microsoft/signalr",
          root: "signalR"
        }
      }
    },
    baseExport
  ),
  Object.assign(
    {
      mode: "production",
      entry: {
        "dotnetify-vue": "./src/dotnetify/vue/index.js",
        "dotnetify-vue.min": "./src/dotnetify/vue/index.js"
      },
      output: {
        path: __dirname + "/dist/dist",
        library: "dotnetify",
        libraryTarget: "umd"
      },
      externals: {
        vue: {
          commonjs: "vue",
          commonjs2: "vue",
          amd: "vue",
          root: "Vue"
        },
        "@microsoft/signalr": {
          commonjs: "@microsoft/signalr",
          commonjs2: "@microsoft/signalr",
          amd: "@microsoft/signalr",
          root: "signalR"
        }
      }
    },
    baseExport
  )
];

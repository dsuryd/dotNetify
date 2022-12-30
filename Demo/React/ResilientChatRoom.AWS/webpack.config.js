"use strict";

const webpack = require("webpack");
require("dotenv").config({ path: "./.env" });

module.exports = {
  mode: "development",
  entry: { main: "./src/index" },
  output: {
    path: __dirname + "/wwwroot/dist",
    publicPath: "/dist/"
  },
  devtool: "source-map",
  resolve: {
    extensions: [".tsx", ".ts", ".js"],
    modules: ["src", "node_modules"]
  },
  module: {
    rules: [
      { test: /\.jsx?$/, use: "babel-loader", exclude: /node_modules/ },
      { test: /\.tsx?$/, use: "ts-loader", exclude: /node_modules/ }
    ]
  },
  plugins: [new webpack.DefinePlugin({ "process.env": JSON.stringify(process.env) })]
};

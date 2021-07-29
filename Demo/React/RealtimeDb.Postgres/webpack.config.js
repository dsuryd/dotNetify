"use strict";

module.exports = {
  mode: "development",
  entry: { main: "./src/index" },
  output: {
    path: __dirname + "/wwwroot/dist",
    publicPath: "/dist/"
  },
  devtool: "source-map",
  resolve: {
    extensions: [".tsx", ".jsx", ".ts", ".js"],
    modules: ["src", "node_modules"]
  },
  module: {
    rules: [
      { test: /\.jsx?$/, use: "babel-loader", exclude: /node_modules/ },
      { test: /\.tsx?$/, use: "ts-loader", exclude: /node_modules/ }
    ]
  }
};

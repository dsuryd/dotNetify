"use strict";
let webpack = require('webpack');
module.exports = {
   entry: "./wwwroot/lib/app.js",
   output: {
      filename: "wwwroot/lib/bundle.min.js"
   },
   resolve: {
      modules: ["wwwroot/lib", "node_modules"],
      alias: {
         signalR: "@aspnet/signalr-client/dist/browser/signalr-clientES5-1.0.0-alpha2-final"
         //signalR: "./no-jquery.signalR"
      }
   },
   //plugins: [
   //   new webpack.optimize.UglifyJsPlugin({ minimize: true })
   //]
};
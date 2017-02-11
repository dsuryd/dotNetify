"use strict";
let webpack = require( 'webpack' );
module.exports = {
   entry: "./wwwroot/lib/app.js",
   output: {
      filename: "wwwroot/lib/bundle.min.js"
   },
   resolve: {
      modules: ["wwwroot/lib", "node_modules"],
      alias: { "pathjs": "path.min"}
   },
   plugins: [
    new webpack.optimize.UglifyJsPlugin( { minimize: true } )
   ]
};
/*
This file in the main entry point for defining grunt tasks and using grunt plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkID=513275&clcid=0x409
*/
module.exports = function (grunt) {
   grunt.initConfig({
      clean: ["wwwroot/lib/jquery.*", "wwwroot/lib/knockout*.*", "wwwroot/lib/require.*"],
      copy: {
         main: {
            files: [
                {
                   src: "node_modules/jquery/dist/jquery.min.js",
                   dest: "wwwroot/lib",
                   expand: true,
                   filter: "isFile",
                   flatten: true
                },
               {
                  src: "node_modules/jquery.ui.widget/jquery.ui.widget.js",
                  dest: "wwwroot/lib",
                  expand: true,
                  filter: "isFile",
                  flatten: true
               },
               {
                  src: "node_modules/knockout/build/output/knockout-latest.js",
                  dest: "wwwroot/lib",
                  expand: true,
                  filter: "isFile",
                  flatten: true
               },
               {
                  src: "node_modules/knockout-mapping/dist/knockout.mapping.min.js",
                  dest: "wwwroot/lib",
                  expand: true,
                  filter: "isFile",
                  flatten: true
               },
               {
                  src: "node_modules/signalr/jquery.signalr.js",
                  dest: "wwwroot/lib",
                  expand: true,
                  filter: "isFile",
                  flatten: true
               },
               {
                  src: "node_modules/requirejs/require.js",
                  dest: "wwwroot/lib",
                  expand: true,
                  filter: "isFile",
                  flatten: true
               },
            ]
         }
      },
      uglify: {
         build: {
            files: [
               {
                  src: "wwwroot/lib/require.js",
                  dest: "wwwroot/lib/require.min.js"
               },
               {
                  src: "wwwroot/lib/jquery.ui.widget.js",
                  dest: "wwwroot/lib/jquery.ui.widget.min.js"
               }
            ]
         }
      }
   });
   grunt.loadNpmTasks("grunt-contrib-clean");
   grunt.loadNpmTasks("grunt-contrib-copy");
   grunt.loadNpmTasks("grunt-contrib-uglify");
   grunt.registerTask("all", ["clean", "copy", "uglify"]);
};
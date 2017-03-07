/*
This file in the main entry point for defining grunt tasks and using grunt plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkID=513275&clcid=0x409
*/
module.exports = function (grunt) {
   grunt.initConfig({
      clean: ["wwwroot/lib/jquery.*", "wwwroot/lib/knockout-latest.*", "wwwroot/lib/require.*"],
      copy: {
         build: {
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
         },
         dist: {
            files: [
               {
                  src: "wwwroot/lib/dotnetify.js",
                  dest: "../dist/dotnetify.js"
               },
               {
                  src: "wwwroot/lib/dotnetify.router.js",
                  dest: "../dist/dotnetify.router.js"
               },
               {
                  src: "../WebApplication.Core.React/wwwroot/lib/dotnetify-react.js",
                  dest: "../dist/dotnetify-react.js"
               },
               {
                  src: "../WebApplication.Core.React/wwwroot/lib/dotnetify-react.router.js",
                  dest: "../dist/dotnetify-react.router.js"
               },
               {
                  src: "wwwroot/lib/knockout.mapping-latest.js",
                  dest: "../dist/knockout.mapping-latest.js"
               }
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
         },
         dist: {
            files: [
               {
                  src: "wwwroot/lib/dotnetify.js",
                  dest: "../dist/dotnetify.min.js"
               },
               {
                  src: "wwwroot/lib/dotnetify.router.js",
                  dest: "../dist/dotnetify.router.min.js"
               },
               {
                  src: "../WebApplication.Core.React/wwwroot/lib/dotnetify-react.js",
                  dest: "../dist/dotnetify-react.min.js"
               },
               {
                  src: "../WebApplication.Core.React/wwwroot/lib/dotnetify-react.router.js",
                  dest: "../dist/dotnetify-react.router.min.js"
               },
            ]

         }
      }
   });
   grunt.loadNpmTasks("grunt-contrib-clean");
   grunt.loadNpmTasks("grunt-contrib-copy");
   grunt.loadNpmTasks("grunt-contrib-uglify");
   grunt.registerTask("build", ["clean", "copy:build", "uglify:build"]);
   grunt.registerTask("dist", ["copy:dist", "uglify:dist"]);
};
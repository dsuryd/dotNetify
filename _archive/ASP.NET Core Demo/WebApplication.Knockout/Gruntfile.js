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
                  dest: "../WebApplication.Knockout/wwwroot/lib",
                  expand: true,
                  filter: "isFile",
                  flatten: true
               },
               {
                  src: "node_modules/jquery.ui.widget/jquery.ui.widget.js",
                  dest: "../WebApplication.Knockout/wwwroot/lib",
                  expand: true,
                  filter: "isFile",
                  flatten: true
               },
               {
                  src: "node_modules/knockout/build/output/knockout-latest.js",
                  dest: "../WebApplication.Knockout/wwwroot/lib",
                  expand: true,
                  filter: "isFile",
                  flatten: true
               },
               {
                  src: "node_modules/signalr/jquery.signalr.js",
                  dest: "../WebApplication.Knockout/wwwroot/lib",
                  expand: true,
                  filter: "isFile",
                  flatten: true
               },
               {
                  src: "node_modules/@aspnet/signalr/dist/browser/signalr.js",
                  dest: "../WebApplication.Knockout/wwwroot/lib",
                  expand: true,
                  filter: "isFile",
                  flatten: true
               },
               {
                  src: "node_modules/requirejs/require.js",
                  dest: "../WebApplication.Knockout/wwwroot/lib",
                  expand: true,
                  filter: "isFile",
                  flatten: true
               },
               {
                  src: "../../src/dotnetify-hub.js",
                  dest: "../WebApplication.Knockout/wwwroot/lib",
                  expand: true,
                  filter: "isFile",
                  flatten: true
               },
               {
                  src: "../../src/knockout/*.js",
                  dest: "../WebApplication.Knockout/wwwroot/lib",
                  expand: true,
                  filter: "isFile",
                  flatten: true
               },
               {
                  src: "../../src/dotnetify-hub.js",
                  dest: "../../ASP.NET Demo/WebApplication.Knockout/Scripts",
                  expand: true,
                  filter: "isFile",
                  flatten: true
               },
               {
                  src: "../../src/knockout/*.js",
                  dest: "../../ASP.NET Demo/WebApplication.Knockout/Scripts",
                  expand: true,
                  filter: "isFile",
                  flatten: true
               },
               {
                  src: "../../src/*.js",
                  dest: "../WebApplication.React/wwwroot/lib",
                  expand: true,
                  filter: "isFile",
                  flatten: true
               },
               {
                  src: "../../src/react/*.js",
                  dest: "../WebApplication.React/wwwroot/lib",
                  expand: true,
                  filter: "isFile",
                  flatten: true
               },
               {
                  src: "../../src/*.js",
                  dest: "../WebApplication.React/wwwroot/lib",
                  expand: true,
                  filter: "isFile",
                  flatten: true
               },
            ]
         },
         dist: {
            files: [
               {
                  src: "../../src/knockout/dotnetify.js",
                  dest: "../../dist/dotnetify.js"
               },
               {
                  src: "../../src/knockout/dotnetify.router.js",
                  dest: "../../dist/dotnetify.router.js"
               },
               {
                  src: "wwwroot/lib/knockout.mapping-latest.js",
                  dest: "../../dist/knockout.mapping-latest.js"
               },
               {
                  src: "../../src/dotnetify-hub.js",
                  dest: "../../dist/dotnetify-hub.js"
               },
               {
                  src: "../../src/react/dotnetify-react.js",
                  dest: "../../dist/dotnetify-react.js"
               },
               {
                  src: "../../src/react/dotnetify-react.router.js",
                  dest: "../../dist/dotnetify-react.router.js"
               },
               {
                  src: "../../src/react/dotnetify-react.scope.js",
                  dest: "../../dist/dotnetify-react.scope.js"
               },
               {
                  src: "../../src/jquery-shim.js",
                  dest: "../../dist/jquery-shim.js"
               },
               {
                  src: "../../src/signalR-netfx.js",
                  dest: "../../dist/signalR-netfx.js"
               },
               {
                  src: "../../src/signalR-netcore.js",
                  dest: "../../dist/signalR-netcore.js"
               },
               {
                  src: "../../src/typings/dotnetify.d.ts",
                  dest: "../../dist/typings/dotnetify.d.ts"
               },
               {
                  src: "../../src/typings/dotnetify-react.router.d.ts",
                  dest: "../../dist/typings/dotnetify-react.router.d.ts"
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
                  src: [
                     "../../src/knockout/dotnetify.js",
                     "../../src/knockout/dotnetify.router.js"
                  ],
                  dest: "../../dist/dotnetify.min.js"
               },
               {
                  src: [
                     "../../src/dotnetify-hub.js",
                     "../../src/react/dotnetify-react.js"
                  ],
                  dest: "../../dist/dotnetify-react.min.js"
               },
               {
                  src: "../../src/signalR-netcore.js",
                  dest: "../../dist/signalR-netcore.min.js"
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
require.config({
   baseUrl: '/lib',
   paths: {
      "jquery": "jquery.min",
      "jquery-ui": "jquery.ui.widget",
      "knockout": "knockout-latest",
      "ko-mapping": "knockout.mapping-latest",
      "dotnetify": "dotnetify",
      "router": "dotnetify.router",
      "signalr": "jquery.signalR",
      "signalr-hub": "dotnetify-hub",
      "path": "path.min",
   },
   shim: {
      "jquery": { exports: "$" },
      "jquery-ui": ["jquery"],
      "knockout": { exports: "ko" },
      "path": { exports: "Path" },
      "router": ["path"],
      "signalr": { deps: ["jquery"], exports: "$.connection" },
      "signalr-hub": ["signalr"]
   }
});

require(['jquery', 'dotnetify', 'router'], function ($) {
   $(function () {
   });
});
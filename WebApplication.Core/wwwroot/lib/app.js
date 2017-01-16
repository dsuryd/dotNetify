require.config({
   baseUrl: '/lib',
   paths: {
      "jquery": "jquery.min",
      "jquery-ui": "jquery.ui.widget",
      "knockout": "knockout-latest",
      "ko-mapping": "knockout.mapping.min",
      "dotnetify": "dotnetify",
      "signalr": "jquery.signalR",
      "signalr-hub": "dotnetify-hub",
      "path": "path.min",
   },
   shim: {
      "jquery": { exports: "$" },
      "jquery-ui": ["jquery"],
      "knockout": { exports: "ko" },
      "path": { exports: "Path" },
      "signalr": { deps: ["jquery"], exports: "$.connection" },
      "signalr-hub": ["signalr"]
   }
});

require(['jquery', 'dotnetify', 'path'], function ($) {
   $(function () {
   });
});
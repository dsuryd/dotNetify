require.config({
   baseUrl: '/lib',
   paths: {
      "jquery": "jquery.min",
      "jquery-ui": "jquery.ui.widget",
      "knockout": "knockout-latest",
      "ko-mapping": "knockout.mapping-latest",
      "dotnetify": "dotnetify",
      "router": "dotnetify.router",
      "signalr": "jquery.signalR"
   },
   shim: {
      "jquery": { exports: "$" },
      "jquery-ui": ["jquery"],
      "knockout": { exports: "ko" },
      "signalr": { deps: ["jquery"], exports: "$.connection" }
   }
});

require(['jquery', 'dotnetify', 'router'], function ($) {
   $(function () {
   });
});
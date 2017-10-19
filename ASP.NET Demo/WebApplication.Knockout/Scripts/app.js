require.config({
   baseUrl: '/Scripts',
   paths: {
      "jquery": "jquery-1.11.3.min",
      "jquery-ui": "jquery-ui-widget-1.11.4.min",
      "knockout": "knockout-3.4.1",
      "ko-mapping": "knockout.mapping-latest",
      "dotnetify": "dotnetify",
      "router": "dotnetify.router",
      "signalR": "jquery.signalR-2.2.2.min"
   },
   shim: {
      "jquery": { exports: "$" },
      "knockout": { exports: "ko" },
      "signalr": { deps: ["jquery"], exports: "$.connection" }
   }
});

require(['jquery', 'dotnetify', 'router'], function ($) {
   $(function () {
   });
});
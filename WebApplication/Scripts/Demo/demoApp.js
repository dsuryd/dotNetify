require.config({
   baseUrl: '/Scripts',
   paths: {
      "jquery": "jquery-1.11.3.min",
      "jquery-ui": "jquery-ui-widget-1.11.4.min",
      "knockout": "knockout-3.3.0",
      "ko-mapping": "knockout.mapping-latest",
      "dotnetify": "dotnetify",
      "router": "dotnetify.router",
      "path": "path.min",
      "signalr": "jquery.signalR-2.2.0.min",
      "signalr-hub": "/signalr/hubs?",
      "bootstrap": "DemoLibs/bootstrap.min",
      "calendar": "DemoLibs/zabuto_calendar.min",
   },
   shim: {
      "jquery": { exports: "$" },
      "knockout": { exports: "ko" },
      "path": { exports: "Path" },
      "bootstrap": ["jquery"],
      "calendar": ["jquery"],
      "signalr": { deps: ["jquery"], exports: "$.connection" },
      "signalr-hub": ["signalr"]
   }
});

require(['jquery', 'knockout', 'dotnetify', 'router', 'path', 'bootstrap', 'calendar'], function ($) {
   $(function () {
      dotnetify.debug = true;
   });
});
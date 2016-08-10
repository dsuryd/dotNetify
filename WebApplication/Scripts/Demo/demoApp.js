require.config({
   baseUrl: '/Scripts',
   paths: {
      "jquery": "jquery-1.11.3.min",
      "jquery-ui": "jquery-ui-widget-1.11.4.min",
      "knockout": "knockout-3.3.0",
      "ko-mapping": "knockout.mapping-latest",
      "dotnetify": "dotnetify",
      "router": "dotnetify.router",
      "binder": "dotnetify.binder",
      "binder-polymer": "dotnetify.binder.polymer",
      "path": "path.min",
      "signalr": "jquery.signalR-2.2.0.min",
      "signalr-hub": "/signalr/hubs?",
      "bootstrap": "DemoLibs/bootstrap.min"
   },
   shim: {
      "jquery": { exports: "$" },
      "knockout": { exports: "ko" },
      "path": { exports: "Path" },
      "router": ["path"],
      "bootstrap": ["jquery"],
      "signalr": { deps: ["jquery"], exports: "$.connection" },
      "signalr-hub": ["signalr"],
      "binder-polymer": ["binder"]
   }
});

require(['jquery', 'knockout', 'dotnetify', 'router', 'binder-polymer', 'bootstrap'], function ($, ko) {
   $(function () {
      dotnetify.debug = true;
   });
});
require.config({
   baseUrl: '/lib',
   paths: {
      "jquery": "jquery.min",
      "jquery-ui": "jquery.ui.widget",
      "knockout": "knockout-latest",
      "ko-mapping": "knockout.mapping.min",
      "dotnetify": "dotnetify",
      "router": "dotnetify.router",
      "binder": "dotnetify.binder",
      "binder-polymer": "dotnetify.binder.polymer",
      "path": "path.min",
      "signalr": "jquery.signalR",
      "signalr-hub": "dotnetify-hub",
      "bootstrap": "demo/bootstrap.min"
   },
   shim: {
      "jquery": { exports: "$" },
      "jquery-ui": ["jquery"],
      "knockout": { exports: "ko" },
      "path": { exports: "Path" },
      "router": ["path"],
      "bootstrap": ["jquery"],
      "signalr": { deps: ["jquery"], exports: "$.connection" },
      "signalr-hub": ["signalr"],
      "binder-polymer": ["binder"]
   }
});

// Specify specific transports for SignalR.
require(['signalr-hub'], function () {
   dotnetify.hubOptions = { "transport": ["webSockets", "longPolling"] };
});

require(['jquery', 'knockout', 'dotnetify', 'router', 'bootstrap'], function ($, ko) {
   $(function () {
      dotnetify.debug = true;
   });
});
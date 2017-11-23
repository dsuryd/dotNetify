require.config({
   baseUrl: '/Scripts',
   paths: {
      "jquery": "jquery-1.11.3.min",
      "jquery-ui": "jquery-ui-widget-1.11.4.min",
      "knockout": "knockout-3.4.1",
      "ko-mapping": "knockout.mapping-latest",
      "dotnetify": "dotnetify",
      "router": "dotnetify.router",
      "binder": "dotnetify.binder",
      "binder-polymer": "dotnetify.binder.polymer",
      "signalR": "jquery.signalR-2.2.2.min",
      "bootstrap": "DemoLibs/bootstrap.min"
   },
   shim: {
      "jquery": { exports: "$" },
      "knockout": { exports: "ko" },
      "bootstrap": ["jquery"],
      "signalR": { deps: ["jquery"], exports: "$.connection" },
      "binder-polymer": ["binder"]
   }
});

require(['jquery', 'knockout', 'dotnetify', 'router', 'binder-polymer', 'bootstrap'], function ($, ko) {
   $(function () {
      dotnetify.debug = true;
   });
});
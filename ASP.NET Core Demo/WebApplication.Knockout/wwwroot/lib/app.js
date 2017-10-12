require.config({
   baseUrl: '/lib',
   paths: {
      "jquery": "jquery.min",
      "jquery-ui": "jquery.ui.widget",
      "knockout": "knockout-latest",
      "ko-mapping": "knockout.mapping-latest",
      "dotnetify": "dotnetify",
      "router": "dotnetify.router",
      "signalr": "signalr-clientES5-1.0.0-alpha2-final"
   },
   shim: {
      "jquery": { exports: "$" },
      "jquery-ui": ["jquery"],
      "knockout": { exports: "ko" }
   }
});

require(['jquery', 'dotnetify', 'router'], function ($) {
   $(function () {
   });
});
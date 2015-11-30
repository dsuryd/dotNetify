require.config({
    baseUrl: '/Scripts',
    paths: {
        "jquery": "jquery-1.11.3.min",
        "jquery-ui": "jquery-ui-widget-1.11.4.min",
        "knockout": "knockout-3.3.0",
        "ko-mapping": "knockout.mapping-latest",
        "ko-dotnetify": "knockout.dotnetify",
        "signalr": "jquery.signalR-2.2.0.min",
        "signalr-hub": "/signalr/hubs?",
        "bootstrap": "DemoLibs/bootstrap.min",
        "chart": "DemoLibs/chart.min",
        "path": "DemoLibs/path.min",
        "calendar": "DemoLibs/zabuto_calendar.min",
        "demo-chart": "Demo/Chart",
        "demo-dashboard": "Demo/Dashboard"
    },
    shim: {
        "jquery": {
            exports: "$"
        },
        "knockout": {
            exports: "ko"
        },
        "bootstrap": {
            deps: ["jquery"]
        },
        "calendar": {
            deps: ["jquery"]
        },
        "signalr": {
            deps: ["jquery"],
            exports: "$.connection"
        },
        "signalr-hub": {
            deps: ["signalr"],
        }
    }
});

require(['jquery', 'knockout', 'bootstrap', 'ko-dotnetify', 'path', 'calendar', 'chart', 'demo-chart', 'demo-dashboard'], function ($) {
    $(function () {
        dotnetify.debug = true;
    });
});
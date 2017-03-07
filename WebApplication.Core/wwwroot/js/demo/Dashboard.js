var $;
var DashboardPanelVM = (function () {
    function DashboardPanelVM() {
    }
    DashboardPanelVM.prototype.clearAll = function () { $("#DisplayArea").empty(); };
    DashboardPanelVM.prototype.loadWidget = function () {
        var vm = this;
        var newWidget = vm.NewWidget();
        if (newWidget == null)
            return;
        var widgetId = newWidget.Id();
        var widgetView = newWidget.ViewName();
        // Add a new div for a new widget inside the designated area.
        $("#DisplayArea").append("<div id=widget" + widgetId + "></div>");
        // Load the HTML view of the new widget and put it inside the new div.
        vm.$loadView("#DisplayArea #widget" + widgetId, "/Demo/" + widgetView, function () {
            // Load the view model(s) for the new widget view.
            // Since there can be multiple widgets of the same type, we're going to use the widget id 
            // to make a unique identification by setting it on the data-vm-id attribute.
            $(this).find("[data-vm]").attr("data-vm-id", widgetId);
            // Scroll to bottom.
            $(".panel-main").animate({ scrollTop: $("#DisplayArea div:last").offset().top }, 500);
        });
    };
    return DashboardPanelVM;
}());

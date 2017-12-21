var JobQueueVM = (function () {
    function JobQueueVM() {
        this._jobID = "";
        this._jobOutput = "";
    }
    JobQueueVM.prototype.$ready = function () {
        var _this = this;
        var vm = this;
        vm.$on(vm.JobComplete, function (iArgs) { return _this.onJobComplete(iArgs); });
    };
    JobQueueVM.prototype.add = function () {
        var vm = this;
        vm.NewJob({ ID: vm._jobID(), Start: new Date().toLocaleTimeString() });
        vm._jobID(null);
    };
    JobQueueVM.prototype.onJobComplete = function (iEventArgs) {
        var vm = this;
        vm._jobOutput(vm._jobOutput() + "<br/>" + iEventArgs);
    };
    return JobQueueVM;
}());

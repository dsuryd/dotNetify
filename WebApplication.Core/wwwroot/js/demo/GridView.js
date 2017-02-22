var $;
var GridViewVM = (function () {
    function GridViewVM() {
    }
    // Inject local observables into each ListHeader's item to implement sorting.
    GridViewVM.prototype._injectSortOrders = function () {
        var vm = this;
        vm.$inject(vm.ListHeader, {
            _sortOrder: 0,
            _sortDesc: function () { return this._sortOrder() == 2; },
            _sortAsc: function () { return this._sortOrder() == 1; }
        });
    };
    // Sort the list.
    GridViewVM.prototype.sort = function (iItem) {
        var vm = this;
        // Clear the sort order of other list header.
        $.each(vm.ListHeader(), function (idx, header) { if (header != iItem)
            header._sortOrder(0); });
        // Toggle sort order.
        iItem._sortOrder(iItem._sortOrder() == 2 ? 1 : 2);
        // Sorting will trigger unnecessary change event to the server.  This can be prevented by
        // doing the operation inside $preventBinding.
        vm.$preventBinding(function () {
            var prop = iItem.Id();
            var sortValue = iItem._sortDesc() ? 1 : -1;
            vm.Employees.sort(function (i, j) {
                var left = i[prop](), right = j[prop]();
                return left == right ? 0 : left < right ? -sortValue : sortValue;
            });
        });
    };
    // Inject local observables into each Employees's item to implement inline editing.
    GridViewVM.prototype._injectEditFlags = function () {
        var vm = this;
        vm.$inject(vm.Employees, {
            _editFirstName: false,
            _editLastName: false,
            _isActive: function () { return vm.SelectedId() == this.Id(); },
            editFirstName: function (iItem) { return iItem._editFirstName(true); },
            editLastName: function (iItem) { return iItem._editLastName(true); }
        });
    };
    // Select a list item.
    GridViewVM.prototype.select = function (iItem) { this.SelectedId(iItem.Id()); };
    return GridViewVM;
}());

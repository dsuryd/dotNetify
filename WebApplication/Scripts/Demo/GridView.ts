class GridViewVM {

   // Inject local observables into each ListHeader's item to implement sorting.
   _injectSortOrders() {
      var vm: any = this;
      vm.$inject(vm.ListHeader, {
         _sortOrder: 0,
         _sortDesc: function () { return this._sortOrder() == 2 },
         _sortAsc: function () { return this._sortOrder() == 1 }
      });
   }

   // Sort the list.
   sort(iItem) {
      var vm: any = this;

      // Clear the sort order of other list header.
      $.each(vm.ListHeader(), (idx, header) => { if (header != iItem) header._sortOrder(0) });

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
         })
      });
   }

   // Inject local observables into each Employees's item to implement inline editing.
   _injectEditFlags() {
      var vm: any = this;
      vm.$inject(vm.Employees, {
         _editFirstName: false,
         _editLastName: false,
         _isActive: function () { return vm.SelectedId() == this.Id() },
         editFirstName: iItem => iItem._editFirstName(true),
         editLastName: iItem => iItem._editLastName(true) 
      });
   }

   // Select a list item.
   select(iItem) { (<any>this).SelectedId(iItem.Id()); }
}

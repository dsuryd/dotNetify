var TreeViewVM = (function () {
    function TreeViewVM() {
    }
    TreeViewVM.prototype.isSelected = function (iItem) { return this.SelectedId() == iItem.Id(); };
    TreeViewVM.prototype.expandIcon = function (iItem) { return iItem.Expanded() ? 'glyphicon-minus' : 'glyphicon-plus'; };
    TreeViewVM.prototype.expand = function (iItem, iElement, iParentItem) {
        var vm = this;
        if (iItem.Children() == null) {
            // Setting the item's Expanded property to true will cause the server to load the subitems 
            // and send the item with its subitems back through ExpandedItem property.
            iItem.Expanded(true);
            // When the new tree item in the ExpandedItem is received, use it to replace the one here.
            vm.$once(vm.ExpandedItem, function (iNewItem) {
                if (iItem.Id() == iNewItem.Id())
                    iParentItem.Children.replace(iItem, iNewItem);
                // The iNewItem object now belongs to the tree, and must be removed from ExpandedItem.
                vm.ExpandedItem(null);
            });
        }
        else
            // If the subitems are already here, just toggle the expanded state locally.
            vm.$preventBinding(function () { return iItem.Expanded(!iItem.Expanded()); });
    };
    return TreeViewVM;
}());

class LinkedTreeViewVM extends TreeViewVM {

   expand(iItem, iElement, iParentItem) {
      var vm: any = this;

      if (iItem.Children() == null) {

         // Send the item Id to expand to the server through ExpandId property binding.
         vm.ExpandId(iItem.Id());

         // When the new tree item in the ExpandedItem is received, use it to replace the one here.
         vm.$once(vm.ExpandedItem, iNewItem => {
            if (iItem.Id() == iNewItem.Id())
               iParentItem.Children.replace(iItem, iNewItem);

            // The iNewItem object now belongs to the tree, and must be removed from ExpandedItem.
            vm.ExpandedItem(null);
         });
      }

      // Toggle the expanded state locally.
      vm.$preventBinding(() => iItem.Expanded(!iItem.Expanded()));
   }
}
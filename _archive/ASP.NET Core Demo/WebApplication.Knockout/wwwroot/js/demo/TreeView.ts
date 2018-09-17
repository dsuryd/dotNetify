class TreeViewVM {

   isSelected(iItem) { return (<any>this).SelectedId() == iItem.Id() }
   expandIcon(iItem) { return iItem.Expanded() ? 'glyphicon-minus' : 'glyphicon-plus' }

   expand(iItem, iElement, iParentItem) {
      var vm: any = this;

      if (iItem.Children() == null) {
         // Setting the item's Expanded property to true will cause the server to load the subitems 
         // and send the item with its subitems back through ExpandedItem property.
         iItem.Expanded(true);

         // When the new tree item in the ExpandedItem is received, use it to replace the one here.
         vm.$once(vm.ExpandedItem, iNewItem => {
            if (iItem.Id() == iNewItem.Id())
               iParentItem.Children.replace(iItem, iNewItem);

            // The iNewItem object now belongs to the tree, and must be removed from ExpandedItem.
            vm.ExpandedItem(null);
         });
      }
      else
         // If the subitems are already here, just toggle the expanded state locally.
         vm.$preventBinding(() => iItem.Expanded(!iItem.Expanded()));
   }
}
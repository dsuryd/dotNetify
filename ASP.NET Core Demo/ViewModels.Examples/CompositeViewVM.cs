using DotNetify;

namespace ViewModels
{
   /// <summary>
   /// This examples demonstrates how to compose a view from multiple sub-views that can communicate with each other.
   /// The CompositeViewVM is the master view model composed of two subordinate view models, GridViewVM and TreeViewVM.
   /// The master view model doesn't have any view, its only purpose is to manage its sub view models, which includes
   /// instantiation and enabling the communication through Observer pattern.
   /// </summary>
   public class CompositeViewVM : BaseVM
   {
      private LinkedGridViewVM _gridViewVM;
      private LinkedTreeViewVM _treeViewVM;

      /// <summary>
      /// A subclass of GridViewVM that hides the SelectedDetails property from the view.
      /// </summary>
      private class LinkedGridViewVM : GridViewVM
      {
         public LinkedGridViewVM(EmployeeModel model) : base(model) { Ignore(() => SelectedDetails); }
      }

      /// <summary>
      /// A subclass of TreeViewVM that uses a different binding to expand, 
      /// and provide a new method to expand and select a particular tree item.
      /// </summary>
      private class LinkedTreeViewVM : TreeViewVM
      {
         public int ExpandId
         {
            get { return 0; }
            set { ExpandedItemFunc = () => LoadTreeItem(value); Changed(() => ExpandedItem); }
         }

         public void ExpandTo(int iId)
         {
            if (iId != SelectedId) { SelectedId = iId; Changed(() => Root); }
         }

         public LinkedTreeViewVM(EmployeeModel model) : base(model) { }
      }

      /// <summary>
      /// Constructor.
      /// </summary>
      public CompositeViewVM(EmployeeModel model)
      {
         _gridViewVM = new LinkedGridViewVM(model);
         _treeViewVM = new LinkedTreeViewVM(model);

         _gridViewVM.PropertyChanged += (sender, e) =>
         {
            if (e.PropertyName == "SelectedId")
               _treeViewVM.ExpandTo(_gridViewVM.SelectedId);
         };

         _treeViewVM.PropertyChanged += (sender, e) =>
         {
            if (e.PropertyName == "SelectedId")
               _gridViewVM.SelectedId = _treeViewVM.SelectedId;
         };
      }

      /// <summary>
      /// Override this method to allow the master view model handles instantiation of its sub view models.
      /// This is needed if the master VM needs to perform initialization on its sub VMs prior to binding
      /// with the views.
      /// </summary>
      public override BaseVM GetSubVM(string iVMTypeName)
      {
         if (iVMTypeName == typeof(LinkedGridViewVM).Name)
            return _gridViewVM;
         else if (iVMTypeName == typeof(LinkedTreeViewVM).Name)
            return _treeViewVM;

         return base.GetSubVM(iVMTypeName);
      }
   }
}
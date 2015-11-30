using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using DotNetify;

namespace ViewModels
{
   /// <summary>
   /// This view model demonstrates how to do lazy-loading on a deep tree without storing anything in the view model.
   /// </summary>
   public class TreeViewVM : BaseVM
   {
      /// <summary>
      /// The class that holds a tree item data.
      /// </summary>
      public class TreeItem
      {
         public int Id { get; set; }

         public string Name { get; set; }

         public bool CanExpand { get; set; }

         public bool Expanded { get; set; }

         public List<TreeItem> Children { get; set; }
      }

      /// <summary>
      /// Root tree item.
      /// </summary>
      public TreeItem Root
      {
         get { return GetRoot(); }
      }

      /// <summary>
      /// Identifies the selected tree item.
      /// </summary>
      public int SelectedId
      {
         get { return Get<int>(); }
         set { Set(value); }
      }

      /// <summary>
      /// This property is used to send a newly expanded tree item back to the browser.
      /// Notice that it's using a function delegate to load the data on the fly.
      /// </summary>
      public TreeItem ExpandedItem
      {
         get { return ExpandedItemFunc != null ? ExpandedItemFunc() : null; }
      }

      /// <summary>
      /// The function delegate used to load data on the fly so we won't need to store any data in the view model.
      /// </summary>
      public Func<TreeItem> ExpandedItemFunc { get; set; }

      /// <summary>
      /// Gets the root tree item.
      /// </summary>
      /// <returns></returns>
      private TreeItem GetRoot()
      {
         TreeItem root = null;
         var bigBoss = EmployeeModel.AllRecords.FirstOrDefault(i => i.ReportTo == 0);
         if (bigBoss != null)
            root = LoadTreeItem(bigBoss.Id);

         ExpandToSelection(ref root);
         return root;
      }

      /// <summary>
      /// Expands the root tree item to the currently selected item.
      /// </summary>
      /// <param name="iRoot">Root tree item.</param>
      private void ExpandToSelection(ref TreeItem iRoot)
      {
         if (iRoot == null || SelectedId == 0 || SelectedId == iRoot.Id)
            return;

         var treeItem = LoadTreeItem(SelectedId);

         var records = EmployeeModel.AllRecords;
         var record = records.FirstOrDefault(i => i.Id == SelectedId);
         var reportToId = record.ReportTo;

         // Start from the selected item and work our way up until the root is reached.
         while (reportToId != iRoot.Id && (record = records.FirstOrDefault(i => i.Id == reportToId)) != null)
         {
            var parentItem = LoadTreeItem(record.Id);
            parentItem.Children[parentItem.Children.FindIndex(i => i.Id == treeItem.Id)] = treeItem;
            reportToId = record.ReportTo;
            treeItem = parentItem;
         }
         iRoot.Children[iRoot.Children.FindIndex(i => i.Id == treeItem.Id)] = treeItem;
      }

      /// <summary>
      /// Loads a tree item from an employee record.
      /// </summary>
      /// <param name="iId">Identifies the record.</param>
      /// <returns>Tree item.</returns>
      protected TreeItem LoadTreeItem(int iId)
      {
         // Find the tree item.
         var record = EmployeeModel.AllRecords.FirstOrDefault(i => i.Id == iId);
         if (record == null)
            return null;

         // Find the tree item's children.
         var directReports = EmployeeModel.AllRecords.Where(i => i.ReportTo == iId);
         var treeItem = new TreeItem { Id = record.Id, Name = record.FullName, CanExpand = directReports.Count() > 0, Expanded = true };
         if (directReports.Count() > 0)
         {
            treeItem.Children = new List<TreeItem>();
            foreach (var person in directReports)
            {
               // Check if each child has children to determine whether it can expand.
               bool hasDirectReports = EmployeeModel.AllRecords.Exists(i => i.ReportTo == person.Id);
               treeItem.Children.Add(new TreeItem { Id = person.Id, Name = person.FullName, CanExpand = hasDirectReports });
            }
         }
         return treeItem;
      }

      /// <summary>
      /// Override this method to implement custom resolver for the value update coming from the browser.
      /// VMController doesn't know how to handle a deep tree property path, so we are handling it here.
      /// </summary>
      /// <param name="iVMPath">View model path.</param>
      /// <param name="iValue">Value sent by the browser.</param>
      public override void OnUnresolvedUpdate(string iVMPath, string iValue)
      {
         int id = -1;
         var path = iVMPath.Split('.');
         for (int i = 0; i < path.Length; i++)
         {
            // The $ sign indicates an item key.
            if (path[i].StartsWith("$"))
               int.TryParse(path[i].Trim('$'), out id);
            // If the path ends with Expanded property, then this is a request to expand a tree item.
            else if (path[i] == "Expanded")
            {
               ExpandedItemFunc = () => LoadTreeItem(id);
               Changed(() => ExpandedItem);
            }
         }
      }
   }
}

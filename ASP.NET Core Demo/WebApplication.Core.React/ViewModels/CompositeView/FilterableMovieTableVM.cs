using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using DotNetify;

namespace ViewModels.CompositeView
{
   public class FilterableMovieTableVM : BaseVM
   {
      private Func<IEnumerable<MovieRecord>> _dataSourceFunc;
      private Action _updateData;
      private string _query;

      /// <summary>
      /// Occurs when the movie table selection changes.
      /// </summary>
      public event EventHandler<int> Selected;

      /// <summary>
      /// Function that provides data to the movie table.
      /// </summary>
      public Func<IEnumerable<MovieRecord>> DataSource
      {
         set
         {
            _dataSourceFunc = value;
            _updateData?.Invoke();
         }
      }

      /// <summary>
      /// This method is called when an instance of a view model inside this view model's scope is being created.
      /// It provides a chance for this view model to initialize them.
      /// </summary>
      /// <param name="subVM">Sub-view model instance.</param>
      public override void OnSubVMCreated(BaseVM subVM)
      {
         if (subVM is MovieTableVM)
            InitMovieTableVM(subVM as MovieTableVM);
         else if (subVM is MovieFilterVM)
            InitMovieFilterVM(subVM as MovieFilterVM);
      }

      private void InitMovieTableVM(MovieTableVM vm)
      {
         // Forward the movie table's Selected event.
         vm.Selected += (sender, key) => Selected?.Invoke(this, key);

         // Create an action to update the movie table with the filtered data.
         _updateData = () => vm.DataSource = GetFilteredData;
         _updateData();
      }

      private void InitMovieFilterVM(MovieFilterVM vm)
      {
         // If a filter is added, set the filter query and update the movie table data.
         vm.FilterChanged += (sender, query) =>
         {
            _query = query;
            _updateData?.Invoke();
         };
      }

      /// <summary>
      /// Applies the filter to the data returned by the data source function.
      /// </summary>
      private IEnumerable<MovieRecord> GetFilteredData()
      {
         try
         {
            return !string.IsNullOrEmpty(_query) ?
            _dataSourceFunc().AsQueryable().Where(_query) : _dataSourceFunc();
         }
         catch(Exception)
         {
            return new List<MovieRecord>();
         }
      }
   }
}

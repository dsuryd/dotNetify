using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Exceptions;
using DotNetify;

namespace ViewModels.CompositeView
{
   public class FilterableMovieTableVM : BaseVM
   {
      private Func<IEnumerable<MovieRecord>> _dataSourceFunc;
      private Action _updateData;
      private string _query;

      public event EventHandler<int> Selected;

      public Func<IEnumerable<MovieRecord>> DataSource
      {
         set
         {
            _dataSourceFunc = value;
            _updateData?.Invoke();
         }
      }

      public override void OnSubVMCreated(BaseVM subVM)
      {
         if (subVM is MovieTableVM)
         {
            var vm = subVM as MovieTableVM;
            vm.DataSource = GetFilteredData;
            vm.Selected += (sender, key) => Selected?.Invoke(this, key);
            _updateData = () => vm.DataSource = GetFilteredData;
         }
         else if (subVM is MovieFilterVM)
         {
            var vm = subVM as MovieFilterVM;
            vm.FilterAdded += (sender, query) =>
            {
               _query = query;
               _updateData?.Invoke();
            };
         }
      }

      private IEnumerable<MovieRecord> GetFilteredData()
      {
         if (_dataSourceFunc == null)
            return null;

         return !string.IsNullOrEmpty(_query) ? _dataSourceFunc().AsQueryable().Where(_query) : _dataSourceFunc();
      }
   }
}

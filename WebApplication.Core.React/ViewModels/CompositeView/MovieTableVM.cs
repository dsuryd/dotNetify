using System;
using System.Collections.Generic;
using System.Linq;
using DotNetify;
using ViewModels.Components;

namespace ViewModels.CompositeView
{
   public class MovieTableVM: BaseVM, IPaginatedTable<MovieRecord>
   {
      private int _recordsPerPage = 10;
      private Func<IEnumerable<MovieRecord>> _dataSourceFunc;

      public IEnumerable<string> Headers => new string[] { "Rank", "Movie", "Year", "Cast", "Director" };

      public IEnumerable<MovieRecord> Data => GetData();

      public string ItemKey => nameof(MovieRecord.Rank);

      public int SelectedKey
      {
         get { return Get<int>(); }
         set { Set(value); }
      }
      public int[] Pagination
      {
         get { return Get<int[]>(); }
         set
         {
            Set(value);
            SelectedPage = 1;
         }
      }

      public int SelectedPage
      {
         get { return Get<int>(); }
         set
         {
            Set(value);
            Changed(nameof(Data));
         }
      }

      public void SetDataSource(Func<IEnumerable<MovieRecord>> dataSourceFunc) => _dataSourceFunc = dataSourceFunc;

      private IEnumerable<MovieRecord> GetData()
      {
         var data = _dataSourceFunc();

         if (!data.Any(i => i.Rank == SelectedKey))
            SelectedKey = data.Count() > 0 ? data.First().Rank : -1;

         return Paginate(data);
      }

      /// <summary>
      /// Paginates the query results.
      /// </summary>
      private IEnumerable<MovieRecord> Paginate(IEnumerable<MovieRecord> data)
      {
         // ChangedProperties is a base class property that contains a list of changed properties.
         // Here it's used to check whether user has changed the SelectedPage property value by clicking a pagination button.
         if (this.ChangedProperties.ContainsKey(nameof(SelectedPage)))
            return data.Skip(_recordsPerPage * (SelectedPage - 1)).Take(_recordsPerPage);
         else
         {
            var pageCount = (int)Math.Ceiling(data.Count() / (double)_recordsPerPage);
            Pagination = Enumerable.Range(1, pageCount).ToArray();
            return data.Take(_recordsPerPage);
         }
      }
   }
}

using System;
using System.Collections.Generic;
using System.Linq;
using DotNetify;
using ViewModels.Components;

namespace ViewModels.CompositeView
{
   public class MovieTableVM : BaseVM, IPaginatedTable<MovieRecord>
   {
      private int _recordsPerPage = 10;
      private Func<IEnumerable<MovieRecord>> _dataSourceFunc;

      /// <summary>
      /// Movie table headers.
      /// </summary>
      public IEnumerable<string> Headers => new string[] { "Rank", "Movie", "Year", "Cast", "Director" };

      /// <summary>
      /// Function that provides data to the movie table.
      /// </summary>
      public Func<IEnumerable<MovieRecord>> DataSource
      {
         set
         {
            _dataSourceFunc = value;
            Changed(nameof(Data));
         }
      }

      /// <summary>
      /// Data for the movie table.
      /// </summary>
      public IEnumerable<MovieRecord> Data => GetData();

      /// <summary>
      /// Key property to identify items in the movie table.
      /// </summary>
      public string ItemKey => nameof(MovieRecord.Rank);

      /// <summary>
      /// Key value of the current movie table selection.
      /// </summary>
      public int SelectedKey
      {
         get { return Get<int>(); }
         set
         {
            Set(value);
            Selected?.Invoke(this, value);
         }
      }

      /// <summary>
      /// Pagination count.
      /// </summary>
      public int[] Pagination
      {
         get { return Get<int[]>(); }
         set
         {
            Set(value);
            SelectedPage = 1;
         }
      }

      /// <summary>
      /// Selected pagination page.
      /// </summary>
      public int SelectedPage
      {
         get { return Get<int>(); }
         set
         {
            Set(value);
            Changed(nameof(Data));
         }
      }

      /// <summary>
      /// Occurs when the selection changes.
      /// </summary>
      public event EventHandler<int> Selected;

      /// <summary>
      /// Returns paginated movie data.
      /// </summary>
      private IEnumerable<MovieRecord> GetData()
      {
         if (_dataSourceFunc == null)
            return null;

         var data = _dataSourceFunc();
         if (!data.Any(i => i.Rank == SelectedKey))
            SelectedKey = data.Count() > 0 ? data.First().Rank : -1;

         return Paginate(data);
      }

      /// <summary>
      /// Paginates the given data.
      /// </summary>
      private IEnumerable<MovieRecord> Paginate(IEnumerable<MovieRecord> data)
      {
         // ChangedProperties is a base class property that contains a list of changed properties.
         // Here it's used to check whether user has changed the SelectedPage property value by clicking a pagination button.
         if (this.HasChanged(nameof(SelectedPage)))
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

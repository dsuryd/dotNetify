using System;
using System.Collections.Generic;
using System.Linq;
using DotNetify;

namespace DevApp.ViewModels
{
   public interface IPaginatedTable<T>
   {
      IEnumerable<string> Headers { get; }
      IEnumerable<T> Data { get; }
      int SelectedKey { get; set; }
      int[] Pagination { get; }
      int SelectedPage { get; set; }
   }

   public class MovieTableVM : BaseVM, IPaginatedTable<MovieRecord>
   {
      private int _recordsPerPage = 10;
      private Func<IEnumerable<MovieRecord>> _dataSourceFunc;

      public IEnumerable<string> Headers => new string[] { "Rank", "Movie", "Year", "Director" };

      public Func<IEnumerable<MovieRecord>> DataSource
      {
         set
         {
            _dataSourceFunc = value;
            Changed(nameof(Data));
         }
      }

      public IEnumerable<MovieRecord> Data => GetData();

      public int SelectedKey
      {
         get => Get<int>();
         set
         {
            Set(value);
            Selected?.Invoke(this, value);
         }
      }

      public int[] Pagination
      {
         get => Get<int[]>();
         set
         {
            Set(value);
            SelectedPage = 1;
         }
      }

      public int SelectedPage
      {
         get => Get<int>();
         set
         {
            Set(value);
            Changed(nameof(Data));
         }
      }

      public event EventHandler<int> Selected;

      private IEnumerable<MovieRecord> GetData()
      {
         if (_dataSourceFunc == null)
            return null;

         var data = _dataSourceFunc();
         if (!data.Any(i => i.Rank == SelectedKey))
            SelectedKey = data.Count() > 0 ? data.First().Rank : -1;

         return Paginate(data);
      }

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
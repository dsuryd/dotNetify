using System.Collections.Generic;

namespace ViewModels.Components
{
   public interface IPaginatedTable<T>
    {
      IEnumerable<string> Headers { get; }

      IEnumerable<T> Data { get; }

      string ItemKey { get; }

      int SelectedKey { get; set; }

      int[] Pagination { get; }

      int SelectedPage { get; set; }
   }
}

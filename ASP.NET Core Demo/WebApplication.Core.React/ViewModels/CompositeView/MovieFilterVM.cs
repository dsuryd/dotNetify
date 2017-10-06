using System;
using System.Collections.Generic;
using System.Linq;
using DotNetify;

namespace ViewModels.CompositeView
{
   public class MovieFilterVM : BaseVM
   {
      private List<MovieFilter> _filters = new List<MovieFilter>();

      public class MovieFilter
      {
         public int Id { get; set; }
         public string Property { get; set; }
         public string Operation { get; set; }
         public string Text { get; set; }

         public string ToQuery()
         {
            if (Operation == "has")
               return Property == "Any" ? $"( Movie + Cast + Director ).toLower().contains(\"{Text.ToLower()}\")"
                  : $"{Property}.toLower().contains(\"{Text.ToLower()}\")";
            else
            {
               int intValue = int.Parse(Text);
               if (Operation == "equals")
                  return $"{Property} == {intValue}";
               else
                  return $"{Property} {Operation} {intValue}";
            }
         }

         public static string BuildQuery(IEnumerable<MovieFilter> filters) => string.Join(" and ", filters.Select(i => i.ToQuery()));
      }

      public Action<MovieFilter> Apply => arg =>
      {
         _filters.Add(arg);
         FilterChanged?.Invoke(this, MovieFilter.BuildQuery(_filters));
      };

      public Action<int> Delete => id =>
      {
         _filters = _filters.Where(i => i.Id != id).ToList();
         FilterChanged?.Invoke(this, MovieFilter.BuildQuery(_filters));
      };

      public event EventHandler<string> FilterChanged;
   }
}

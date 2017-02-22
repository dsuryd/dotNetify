using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
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

         public override string ToString() => $"{Property} {Operation} {Text}";

         public string ToQuery()
         {
            if (Operation == "has")
               return Property == "Any" ? $"( Movie + Cast + Director ).toLower().contains(\"{Text.ToLower()}\")"
                  : $"{Property}.toLower().contains(\"{Text.ToLower()}\")";
            else if (Operation == "equals")
               return $"{Property} == {Text}";
            else
               return $"{Property} {Operation} {Text}";
         }

         public static string BuildQuery(IEnumerable<MovieFilter> filters) => string.Join(" and ", filters.Select(i => i.ToQuery()));
      }

      public ICommand Apply => new Command<MovieFilter>(arg =>
      {
         _filters.Add(arg);
         FilterChanged?.Invoke(this, MovieFilter.BuildQuery(_filters));
      });

      public ICommand Delete => new Command<int>(id =>
      {
         _filters = _filters.Where(i => i.Id != id).ToList();
         FilterChanged?.Invoke(this, MovieFilter.BuildQuery(_filters));
      });

      public event EventHandler<string> FilterChanged;
   }
}

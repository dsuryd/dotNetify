using System;
using System.Windows.Input;
using DotNetify;

namespace ViewModels.CompositeView
{
   public class MovieFilterVM : BaseVM
   {
      public class Criteria
      {
         public string Property { get; set; }
         public string Operation { get; set; }
         public string Text { get; set; }
      }

      public ICommand Filter => new Command<Criteria>(arg => FilterAdded?.Invoke( this, BuildQuery(arg)));

      public event EventHandler<string> FilterAdded;

      private string BuildQuery(Criteria criteria)
      {
         if (criteria.Operation == "has")
            return $"{criteria.Property}.contains(\"{criteria.Text}\")";
         else if (criteria.Operation == "equals")
            return $"{criteria.Property} == {criteria.Text}";
         else
            return $"{criteria.Property} {criteria.Operation} {criteria.Text}";
      }
   }
}

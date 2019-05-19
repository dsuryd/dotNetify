using DotNetify;
using System.Windows.Input;

namespace Blazor.Server
{
   public class CounterVM : BaseVM
   {
      public CounterVM()
      {
         CurrentCount = 100;
      }

      public int CurrentCount
      {
         get => Get<int>();
         set => Set(value);
      }

      public ICommand IncrementCount => new Command(() => CurrentCount++);
   }
}
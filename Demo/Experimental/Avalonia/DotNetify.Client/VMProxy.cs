using System.ComponentModel;
using System.Threading.Tasks;

namespace DotNetify.Client
{
   public abstract class VMProxy<T> : INotifyPropertyChanged
   {
      private readonly IDotNetifyClient _dotNetify;

      public event PropertyChangedEventHandler PropertyChanged = delegate { };

      public VMProxy(IDotNetifyClient dotnetify)
      {
         _dotNetify = dotnetify;
         Task task = InitializeAsync();
      }

      private async Task InitializeAsync()
      {
         await _dotNetify.ConnectAsync(typeof(T).Name, this);
      }
   }
}
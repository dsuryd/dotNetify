using System;
using System.Threading.Tasks;

namespace DotNetify.Client
{
   public interface IUIThreadDispatcher
   {
      Task InvokeAsync(Action action);
   }
}
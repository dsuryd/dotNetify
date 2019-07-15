using Microsoft.JSInterop;
using System;

namespace DotNetify.Blazor
{
   public class JsCallback
   {
      private Action<object> _callback;

      public JsCallback(Action<object> callback)
      {
         _callback = callback;
      }

      [JSInvokable]
      public string Callback(object arg)
      {
         _callback(arg);
         return string.Empty;
      }
   }
}
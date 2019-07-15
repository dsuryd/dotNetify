using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace DotNetify.Blazor
{
   public class ComponentInterop
   {
      protected IJSRuntime _jsRuntime;

      public ComponentInterop(IJSRuntime jsRuntime)
      {
         _jsRuntime = jsRuntime;
      }

      public Task AddEventListenerAsync<TEventArg>(string eventName, ElementRef elementRef, Action<TEventArg> eventCallback)
      {
         var jsCallback = new JsCallback(arg => eventCallback?.Invoke(arg.As<TEventArg>()));
         return _jsRuntime.InvokeAsync<object>("dotnetify_blazor.addEventListener", eventName, elementRef, DotNetObjectRef.Create(jsCallback));
      }

      public Task AddEventListenerAsync<TEventArg>(string eventName, string elementSelector, Action<TEventArg> eventCallback)
      {
         var jsCallback = new JsCallback(arg => eventCallback?.Invoke(arg.As<TEventArg>()));
         return _jsRuntime.InvokeAsync<object>("dotnetify_blazor.addEventListener", eventName, elementSelector, DotNetObjectRef.Create(jsCallback));
      }
   }
}
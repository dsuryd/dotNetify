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

      public async Task AddEventListenerAsync<TEventArg>(string eventName, ElementReference elementRef, Action<TEventArg> eventCallback)
      {
         var jsCallback = new JsCallback(arg => eventCallback?.Invoke(arg.As<TEventArg>()));
         await _jsRuntime.InvokeAsync<object>("dotnetify_blazor.addEventListener", eventName, elementRef, DotNetObjectReference.Create(jsCallback));
      }

      public async Task AddEventListenerAsync<TEventArg>(string eventName, string elementSelector, Action<TEventArg> eventCallback)
      {
         var jsCallback = new JsCallback(arg => eventCallback?.Invoke(arg.As<TEventArg>()));
         await _jsRuntime.InvokeAsync<object>("dotnetify_blazor.addEventListener", eventName, elementSelector, DotNetObjectReference.Create(jsCallback));
      }
   }
}
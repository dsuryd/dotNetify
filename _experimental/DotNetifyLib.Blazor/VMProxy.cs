using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetify.Blazor
{
   public interface IVMProxy : IDisposable
   {
      /// <summary>
      /// Reference to the associated 'd-vm-context' HTML markup.
      /// </summary>
      ElementReference ElementRef { get; set; }

      /// <summary>
      /// Listens to the state change event from the server-side view model.
      /// </summary>
      /// <param name="stateChangeEventCallback">Gets called when the client receives state change from the server-side view model.</param>
      Task HandleStateChangeAsync<TState>(Action<TState> stateChangeEventCallback);

      /// <summary>
      /// Listens to the events from the web component elements under this VM context.
      /// </summary>
      /// <param name="eventCallback">Gets called when an element under this VM context raises an event.</param>
      Task HandleElementEventAsync(Action<ElementEvent> eventCallback);

      /// <summary>
      /// Listens to an event from a DOM element.
      /// </summary>
      /// <typeparam name="TEventArg">Event argument type.</typeparam>
      /// <param name="domElement">Document element.</param>
      /// <param name="eventName">Event name.</param>
      /// <param name="eventHandler">Event callback.</param>
      Task HandleDomEventAsync<TEventArg>(string eventName, ElementReference domElement, Action<TEventArg> eventCallback);

      /// <summary>
      /// Dispatches property value to server-side view model.
      /// </summary>
      /// <param name="propertyName">Name that matches a server-side view model property.</param>
      /// <param name="propertyValue">Value to be dispatched.</param>
      Task DispatchAsync(string propertyName, object propertyValue = null);

      /// <summary>
      /// Disposes the context element.
      /// </summary>
      /// <returns></returns>
      Task DisposeAsync();
   }

   public class VMProxy : ComponentInterop, IVMProxy
   {
      private ElementReference? _vmContextElemRef;
      private HashSet<Delegate> _delegates = new HashSet<Delegate>();

      public ElementReference ElementRef
      {
         get => _vmContextElemRef.Value;
         set => _vmContextElemRef = value;
      }

      public VMProxy(IJSRuntime jsRuntime) : base(jsRuntime)
      {
      }

      public void Dispose()
      {
         _ = DisposeAsync();
      }

      public async Task DisposeAsync()
      {
         await _jsRuntime.InvokeAsync<object>("dotnetify_blazor.removeAllEventListeners", _vmContextElemRef);
      }

      public Task HandleStateChangeAsync<TState>(Action<TState> stateChangeCallback)
      {
         if (!_vmContextElemRef.HasValue)
            throw new ArgumentNullException("ElementRef was not set. Make sure you assign it to the \"ref\" attribute of the \"d-vm-context\" tag.");

         return HandleDomEventAsync("onStateChange", ElementRef, stateChangeCallback);
      }

      public Task HandleElementEventAsync(Action<ElementEvent> eventCallback)
      {
         if (!_vmContextElemRef.HasValue)
            throw new ArgumentNullException("ElementRef was not set. Make sure you assign it to the \"ref\" attribute of the \"d-vm-context\" tag.");

         return HandleDomEventAsync<ElementEvent>("onElementEvent", ElementRef, eventCallback);
      }

      public Task HandleDomEventAsync<TEventArg>(string eventName, ElementReference domElement, Action<TEventArg> eventCallback)
      {
         if (_delegates.Contains(eventCallback))
            return Task.CompletedTask;

         _delegates.Add(eventCallback);
         return AddEventListenerAsync<TEventArg>(eventName, domElement, arg => eventCallback?.Invoke(arg));
      }

      public async Task DispatchAsync(string propertyName, object propertyValue = null)
      {
         var data = new Dictionary<string, object>() { { propertyName, propertyValue } };
         await _jsRuntime.InvokeAsync<object>("dotnetify_blazor.dispatch", _vmContextElemRef, JsonConvert.SerializeObject(data, new JsonSerializerSettings
         {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
         }));
      }
   }
}
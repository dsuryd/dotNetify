/* 
Copyright 2015 Dicky Suryadi

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.Caching;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace DotNetify
{
   /// <summary>
   /// This class is a SignalR hub for communicating with browser clients.
   /// </summary>
   public class DotNetifyHub : Hub
   {
      /// <summary>
      /// View model controllers by the client connection Ids.
      /// </summary>
      private static Lazy<MemoryCache> sControllersCache = new Lazy<MemoryCache>(() => new MemoryCache("DotNetify"));

      /// <summary>
      /// How long to keep a view model controller in memory after it hasn't been accessed for a while.
      /// </summary>
      public static TimeSpan CacheExpiration { get; set; }

      /// <summary>
      /// Delegate to override default static memory caching to store view model controllers.
      /// </summary>
      public static Func<MemoryCache> GetControllersCache { get; set; }

      /// <summary>
      /// Provides access to our hub.
      /// </summary>
      public static IHubContext HubContext
      {
         get { return GlobalHost.ConnectionManager.GetHubContext<DotNetifyHub>(); }
      }

      /// <summary>
      /// Static constructor to initialize default cache expiration.
      /// </summary>
      static DotNetifyHub()
      {
         CacheExpiration = new TimeSpan(0, 20, 0);
      }

      /// <summary>
      /// View model controller associated with this client connection.
      /// </summary>
      public VMController VMController
      {
         get
         {
            var cache = GetControllersCache != null ? GetControllersCache() : sControllersCache.Value;

            var newValue = new Lazy<VMController>();
            var cachedValue = cache.AddOrGetExisting(Context.ConnectionId, newValue, GetCacheItemPolicy()) as Lazy<VMController>;
            return cachedValue == null ? newValue.Value : cachedValue.Value;
         }
      }

      /// <summary>
      /// Handles when a client gets disconnected.
      /// </summary>
      /// <param name="iStopCalled">True, if stop was called on the client closing the connection gracefully;
      /// false, if the connection has been lost for longer than the timeout.</param>
      /// <returns></returns>
      public override Task OnDisconnected(bool iStopCalled)
      {
         var cache = GetControllersCache != null ? GetControllersCache() : sControllersCache.Value;

         // Remove the controller on disconnection.
         if (cache.Contains(Context.ConnectionId))
            cache.Remove(Context.ConnectionId);

         return base.OnDisconnected(iStopCalled);
      }

      /// <summary>
      /// Returns cached item policy for view model controllers.
      /// </summary>
      /// <returns>Cache item policy.</returns>
      private CacheItemPolicy GetCacheItemPolicy()
      {
         return new CacheItemPolicy
         {
            SlidingExpiration = CacheExpiration,
            RemovedCallback = i => ((i.CacheItem.Value as Lazy<VMController>).Value as IDisposable).Dispose()
         };
      }

      #region Client Requests

      /// <summary>
      /// This method is called by browser clients to request view model data.
      /// </summary>
      /// <param name="iVMId">Identifies the view model.</param>
      /// <param name="iVMArg">Optional view model's initialization argument.</param>
      public void Request_VM(string iVMId, object iVMArg)
      {
         try
         {
            Debug.WriteLine(String.Format("[DEBUG] Request_VM: {0} {1}", iVMId, Context.ConnectionId));
            VMController.OnRequestVM(Context.ConnectionId, iVMId, iVMArg);
         }
         catch (Exception ex)
         {
            Debug.Fail(ex.ToString());
         }
      }

      /// <summary>
      /// This method is called by browser clients to update a view model's value.
      /// </summary>
      /// <param name="iVMId">Identifies the view model.</param>
      /// <param name="iVMData">View model update data, where key is the property path and value is the property's new value.</param>
      public void Update_VM(string iVMId, Dictionary<string, object> iVMData)
      {
         try
         {
            Debug.WriteLine(String.Format("[DEBUG] Update_VM: {0} {1} {2}", iVMId, Context.ConnectionId, JsonConvert.SerializeObject(iVMData)));
            VMController.OnUpdateVM(Context.ConnectionId, iVMId, iVMData);
         }
         catch (Exception ex)
         {
            Debug.Fail(ex.ToString());
         }
      }

      /// <summary>
      /// This method is called by browser clients to remove its view model as it's no longer used.
      /// </summary>
      /// <param name="iVMId">Identifies the view model.  By convention, this should match a view model class name.</param>
      public void Dispose_VM(string iVMId)
      {
         try
         {
            VMController.OnDisposeVM(Context.ConnectionId, iVMId);
         }
         catch (Exception ex)
         {
            Debug.Fail(ex.ToString());
         }
      }

      #endregion

      #region Server Responses

      /// <summary>
      /// This method is called by the VMManager to send response back to browser clients.
      /// </summary>
      /// <param name="iConnectionId">Identifies the browser client making prior request.</param>
      /// <param name="iVMId">Identifies the view model.</param>
      /// <param name="iVMData">View model data in serialized JSON.</param>
      public static void Response_VM(string iConnectionId, string iVMId, string iVMData)
      {
         HubContext.Clients.Client(iConnectionId).Response_VM(iVMId, iVMData);
      }

      #endregion
   }
}
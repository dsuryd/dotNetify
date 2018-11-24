/*
Copyright 2018 Dicky Suryadi

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
using System.Threading.Tasks;

namespace DotNetify.Client
{
   /// <summary>
   /// Interface to provide UI thread dispatcher to the dotNetify client instance.
   /// </summary>
   public interface IUIThreadDispatcher
   {
      /// <summary>
      /// Invokes an action on the UI thread.
      /// </summary>
      /// <param name="action">Action to invoke.</param>
      Task InvokeAsync(Action action);
   }

   /// <summary>
   /// Default dispatcher for DI placeholder.
   /// </summary>
   public class DefaultUIThreadDispatcher : IUIThreadDispatcher
   {
      public Task InvokeAsync(Action action)
      {
         action?.Invoke();
         return Task.CompletedTask;
      }
   }
}
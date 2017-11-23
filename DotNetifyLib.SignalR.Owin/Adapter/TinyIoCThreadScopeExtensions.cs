/* 
Copyright 2017 Dicky Suryadi

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
using System.Runtime.Remoting.Messaging;
using TinyIoC;

namespace DotNetify
{
   public static class TinyIoThreadScopeExtensions
   {
      public class ThreadLifetimeProvider : TinyIoCContainer.ITinyIoCObjectLifetimeProvider
      {
         private readonly string id = String.Format("TinyIoc_{0}", Guid.NewGuid());

         public object GetObject() => CallContext.LogicalGetData(id);

         public void SetObject(object value) => CallContext.LogicalSetData(id, value);

         public void ReleaseObject()
         {
            (GetObject() as IDisposable)?.Dispose();
            SetObject(null);
         }
      }

      public static TinyIoCContainer.RegisterOptions AsPerThread(this TinyIoCContainer.RegisterOptions registerOptions)
      {
         return TinyIoCContainer.RegisterOptions.ToCustomLifetimeManager(registerOptions, new ThreadLifetimeProvider(), "per thread");
      }
   }
}

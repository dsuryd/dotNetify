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

namespace DotNetify.Client
{
   /// <summary>
   /// Configuration options that can be sent along with request to connect to a server-side view model.
   /// </summary>
   public class VMConnectOptions
   {
      /// <summary>
      /// Arguments to initialize the view model.
      /// </summary>
      public object VMArg { get; set; }

      /// <summary>
      /// Request headers.
      /// </summary>
      public object Headers { get; set; }
   }
}
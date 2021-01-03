/*
Copyright 2020 Dicky Suryadi

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

namespace DotNetify.Forwarding
{
   public class ForwardingOptions
   {
      /// <summary>
      /// Prevent further message processing in this server after being forwarded.
      /// </summary>
      public bool HaltPipeline { get; set; } = true;

      /// <summary>
      /// Use message pack protocol to connect to the target server.
      /// </summary>
      public bool UseMessagePack { get; set; }

      /// <summary>
      /// Number of connections to the target server. Default to 5.
      /// </summary>
      public int ConnectionPoolSize { get; set; } = 5;
   }
}
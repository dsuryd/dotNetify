/*
Copyright 2021 Dicky Suryadi

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

namespace DotNetify.Postgres
{
   /// <summary>
   /// PostgreSQL database configuration.
   /// </summary>
   public class PostgresConfiguration
   {
      /// <summary>
      /// Database connection string.
      /// </summary>
      public string ConnectionString { get; set; }

      /// <summary>
      /// PostgreSQL publication name.
      /// </summary>
      public string PublicationName { get; set; }

      /// <summary>
      /// PostgreSQL replication slot name.
      /// </summary>
      public string ReplicationSlotName { get; set; }
   }
}
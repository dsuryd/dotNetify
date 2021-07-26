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
   /// A marker interface for communicating database change events.
   /// </summary>
   public interface IDbChangeEvent<TTable>
   {
   }

   public class DbInsertEvent<TTable> : IDbChangeEvent<TTable>
   {
      public TTable Row { get; set; }
   }

   public class DbUpdateEvent<TTable> : IDbChangeEvent<TTable>
   {
      public TTable NewRow { get; set; }
      public TTable OldRow { get; set; }
   }

   public class DbDeleteEvent<TTable> : IDbChangeEvent<TTable>
   {
      public TTable Row { get; set; }
   }
}
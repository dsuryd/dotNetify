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

using System.Collections.Generic;

namespace DotNetify
{
   /// <summary>
   /// Provides serialization for view models.
   /// </summary>
   public interface ISerializer
   {
      /// <summary>
      /// Serializes an object.
      /// </summary>
      /// <param name="instance">Object to serialize.</param>
      /// <param name="ignoredPropertyNames">Names of properties that should not be serialized.</param>
      /// <returns>Serialized string.</returns>
      string Serialize(object instance, List<string> ignoredPropertyNames);
   }

   /// <summary>
   /// Provides deserialization for view models.
   /// </summary>
   public interface IDeserializer
   {
      /// <summary>
      /// Deserializes a property value of an object.
      /// </summary>
      /// <param name="instance">Object to deserialize the property to.</param>
      /// <param name="propertyPath">Property path.</param>
      /// <param name="newValue">New value.</param>
      /// <returns>True if the property value was deserialized.</returns>
      bool Deserialize(object instance, string propertyPath, string newValue);
   }
}

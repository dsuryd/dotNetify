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

namespace DotNetify
{
   /// <summary>
   /// Allows a non-subtype of BaseVM to implement its own serialization.
   /// </summary>
   public interface ISerializable
   {
      /// <summary>
      /// Serializes public properties of the instance.
      /// </summary>
      /// <returns>Serialized string.</returns>
      string Serialize();

      /// <summary>
      /// Serializes public properties of the instance that have been changed since the last serialization.
      /// </summary>
      /// <returns>Serialized string.</returns>
      string SerializeChangedProperties();

      /// <summary>
      /// Deserializes a property value of the instance.
      /// </summary>
      /// <param name="vmPath">View model property path.</param>
      /// <param name="newValue">New value.</param>
      /// <returns>True if the property value was deserialized.</returns>
      bool DeserializeProperty(string vmPath, string newValue);
   }
}

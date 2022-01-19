/*
Copyright 2016 Dicky Suryadi

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
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DotNetify
{
   /// <summary>
   /// Custom view model resolver for JSON serializer.
   /// </summary>
   public class VMContractResolver : DefaultContractResolver
   {
      internal List<string> IgnoredPropertyNames { get; set; }
      private Dictionary<string, string> _itemKeyProps;

      /// <summary>
      /// Converter for properties of ICommand type which simply serializes the value to null.
      /// </summary>
      protected class CommandConverter : JsonConverter
      {
         /// <summary>
         /// Determines whether this instance can convert the specified object type.
         /// </summary>
         public override bool CanConvert(Type objectType) => typeof(ICommand).GetTypeInfo().IsAssignableFrom(objectType);

         /// <summary>
         /// Reads the JSON representation of the object; not used.
         /// </summary>
         public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) => throw new NotImplementedException();

         /// <summary>
         /// Writes null value to the JSON output.
         /// </summary>
         public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => writer.WriteNull();
      }

      /// <summary>
      /// Converter for properties of ReactiveProperty type which serializes the property value.
      /// </summary>
      protected class ReactivePropertyConverter : JsonConverter
      {
         /// <summary>
         /// Determines whether this instance can convert the specified object type.
         /// </summary>
         public override bool CanConvert(Type objectType) => objectType.GetTypeInfo().IsGenericType && typeof(IReactiveProperty).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo().GetGenericTypeDefinition());

         /// <summary>
         /// Reads the JSON representation of the object; not used.
         /// </summary>
         public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) => throw new NotImplementedException();

         /// <summary>
         /// Writes the property value to the JSON output.
         /// </summary>
         public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => serializer.Serialize(writer, (value as IReactiveProperty).Value);
      }

      /// <summary>
      /// Default value provider for serialized properties.
      /// </summary>
      protected class DefaultValueProvider : IValueProvider
      {
         private readonly object _defaultValue;

         public DefaultValueProvider(object defaultValue)
         {
            _defaultValue = defaultValue;
         }

         public object GetValue(object target) => _defaultValue;

         public void SetValue(object target, object value) => throw new NotImplementedException();
      }

      /// <summary>
      /// Overrides this method to exclude properties with [Ignore] attribute or those that are in the given list,
      /// and to handle [ItemKey] attribute.
      /// </summary>
      protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
      {
         var property = base.CreateProperty(member, memberSerialization);

         // Don't serialize properties that are decorated with [Ignore] or whose name are in the given list.
         if (member.GetCustomAttribute(typeof(IgnoreAttribute)) != null)
            property.Ignored = true;
         else if (IgnoredPropertyNames != null && IgnoredPropertyNames.Contains(property.PropertyName))
            property.Ignored = true;
         // Add item key property for properties decorated with [ItemKey].
         else if (member.GetCustomAttribute(typeof(ItemKeyAttribute)) != null)
         {
            var itemKeyAttr = member.GetCustomAttribute<ItemKeyAttribute>();
            _itemKeyProps = _itemKeyProps ?? new Dictionary<string, string>();
            _itemKeyProps[ResolvePropertyName($"{member.Name}_itemKey")] = itemKeyAttr.ItemKey;
         }

         return property;
      }

      /// <summary>
      /// Overrides this method to add new properties when certain attributes are present.
      /// </summary>
      protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
      {
         var result = base.CreateProperties(type, memberSerialization);

         // Add [ItemKey] properties.
         if (_itemKeyProps != null)
         {
            foreach (var prop in _itemKeyProps)
            {
               result.Add(new JsonProperty
               {
                  PropertyType = typeof(string),
                  DeclaringType = type,
                  PropertyName = ResolvePropertyName(prop.Key),
                  ValueProvider = new DefaultValueProvider(prop.Value),
                  Readable = true,
                  Writable = false
               });
            }
            _itemKeyProps = null;
         }

         // Add properties from [Command] methods (only for Knockout binding).
         var commands = type.GetMethods().Where(x => x.GetCustomAttribute<CommandAttribute>() != null);
         foreach (var command in commands)
         {
            result.Add(new JsonProperty
            {
               PropertyType = typeof(string),
               DeclaringType = type,
               PropertyName = ResolvePropertyName(command.Name),
               ValueProvider = new DefaultValueProvider(string.Empty),
               Readable = true,
               Writable = false
            });
         }

         return result;
      }

      /// <summary>
      /// Overrides this method to prevent serialization of ICommand or delegate property type.
      /// </summary>
      protected override JsonConverter ResolveContractConverter(Type objectType)
      {
         if (typeof(ICommand).GetTypeInfo().IsAssignableFrom(objectType))
            return new CommandConverter();
         else if (typeof(IReactiveProperty).GetTypeInfo().IsAssignableFrom(objectType))
            return new ReactivePropertyConverter();
         else
         {
            var typeInfo = objectType.GetTypeInfo();
            if (typeInfo.IsSubclassOf(typeof(MulticastDelegate)))
               return new CommandConverter();
         }
         return base.ResolveContractConverter(objectType);
      }
   }
}
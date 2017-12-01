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
using System.Linq;
using System.Reflection;

namespace DotNetify
{
   /// <summary>
   /// Provides PropertyInfo abstraction for both static and runtime property of a view model.
   /// </summary>
   internal class PropertyInfoHelper
   {
      /// <summary>
      /// Property type.
      /// </summary>
      public Type PropertyType { get; }

      /// <summary>
      /// Property value.
      /// </summary>
      public object Value { get; }

      /// <summary>
      /// Property set method.
      /// </summary>
      public Action<object, object> SetMethod => SetValue;

      /// <summary>
      /// Sets property value.
      /// </summary>
      public Action<object, object> SetValue { get; }

      /// <summary>
      /// Gets property value.
      /// </summary>
      public Func<object, object> GetValue { get; }

      /// <summary>
      /// Constructor for static property.
      /// </summary>
      /// <param name="propertyInfo">Property info.</param>
      protected PropertyInfoHelper(PropertyInfo propertyInfo)
      {
         PropertyType = propertyInfo.PropertyType;
         GetValue = viewModel => propertyInfo.GetValue(viewModel);
         SetValue = (viewModel, value) => propertyInfo.SetValue(viewModel, value);
      }

      /// <summary>
      /// Constructor for runtime reactive property.
      /// </summary>
      /// <param name="reactiveProperty">Reactive property.</param>
      protected PropertyInfoHelper(IReactiveProperty reactiveProperty)
      {
         PropertyType = reactiveProperty.PropertyType;
         GetValue = _ => reactiveProperty.Value;
         SetValue = (_, value) => reactiveProperty.Value = value;
      }

      /// <summary>
      /// Finds the property type definiton of a view model.
      /// </summary>
      /// <param name="viewModel">View model that owns the property.</param>
      /// <param name="propertyName">Property name.</param>
      /// <returns></returns>
      public static PropertyInfoHelper Find(object viewModel, string propertyName)
      {
         var propInfo = viewModel.GetType().GetTypeInfo().GetProperty(propertyName);
         if (propInfo != null)
            return new PropertyInfoHelper(propInfo);
         else if (viewModel is IReactiveProperties)
         {
            var reactiveProp = (viewModel as IReactiveProperties).RuntimeProperties.FirstOrDefault(x => x.Name == propertyName);
            if (reactiveProp != null)
               return new PropertyInfoHelper(reactiveProp);
         }
         return null;
      }
   }
}

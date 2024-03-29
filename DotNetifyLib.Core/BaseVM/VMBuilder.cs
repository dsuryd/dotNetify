﻿/*
Copyright 2022 Dicky Suryadi

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
using System.ComponentModel;
using System.Linq;

namespace DotNetify
{
   public static class VMBuilder
   {
      /// <summary>
      /// Builds a view model instance whose properties are generated from given object's properties.
      /// The object's property value can be primitive value, an observable, or an action delegate.
      /// </summary>
      public static BaseVM Build<T>(object propertySource, IEnumerable<Attribute> customAttributes) where T : BaseVM, new()
      {
         var vm = new T();

         if (customAttributes?.Count() > 0)
            vm.CustomAttributes = customAttributes;

         foreach (var prop in propertySource.GetType().GetProperties())
         {
            var propType = prop.PropertyType;
            var propName = prop.Name;
            var propValue = prop.GetValue(propertySource);

            // If the property value is an observable, convert it into a reactive property that subscribes to that observable.
            if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(IObservable<>))
            {
               var genericArgType = propType.GetGenericArguments().First();

               var addPropertyMethod = vm.GetType().GetMethods().First(m => m.Name == nameof(BaseVM.AddProperty) && m.GetParameters().Length == 1).MakeGenericMethod(genericArgType);
               var reactiveProp = addPropertyMethod.Invoke(vm, new object[] { propName });

               var subscribeToMethod = reactiveProp.GetType().GetMethod(nameof(ReactiveProperty<object>.SubscribeTo));
               subscribeToMethod.Invoke(reactiveProp, new object[] { propValue });

               var propChangedEvent = reactiveProp.GetType().GetEvent(nameof(ReactiveProperty<object>.PropertyChanged));

               PropertyChangedEventHandler pushUpdates = (sender, e) => vm.PushUpdates();
               propChangedEvent.AddEventHandler(reactiveProp, pushUpdates);
            }
            else
            {
               var addPropertyMethod = vm.GetType().GetMethods().First(m => m.Name == nameof(BaseVM.AddProperty) && m.GetParameters().Length == 2).MakeGenericMethod(propType);
               var reactiveProp = addPropertyMethod.Invoke(vm, new object[] { propName, propValue });
            }
         }

         return vm;
      }
   }
}
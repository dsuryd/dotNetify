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

using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace DotNetify
{
   using PropertyDictionary = ConcurrentDictionary<string, object>;
   using PropertyStack = ConcurrentStack<string>;

   /// <summary>
   /// Base class for objects that want to provide notification if their property values changed.
   /// </summary>
   public class ObservableObject : INotifyPropertyChanged, IDisposable
   {
      protected PropertyDictionary _propertyValues = new PropertyDictionary();
      protected PropertyStack _propertyChangedStack = new PropertyStack();

      /// <summary>
      /// Occurs when a property value is changed.
      /// </summary>
      public event PropertyChangedEventHandler PropertyChanged;

      /// <summary>
      /// Occurs when the instance is being disposed.
      /// </summary>
      public event EventHandler Disposed;

      /// <summary>
      /// When disposed, fire the Disposed event.
      /// </summary>
      public virtual void Dispose()
      {
         foreach (IDisposable value in _propertyValues.Values.Where(x => x is IDisposable))
            value.Dispose();

         Disposed?.Invoke(this, null);
      }

      /// <summary>
      /// Property accessor. Use this for observable properties.
      /// </summary>
      /// <typeparam name="T">Property type.</typeparam>
      /// <param name="propertyName">Property name.</param>
      /// <returns>Property value.</returns>
      protected T Get<T>([CallerMemberName] string propertyName = null)
      {
         if (_propertyValues.ContainsKey(propertyName))
            return _propertyValues[propertyName] is IReactiveProperty ? (T) (_propertyValues[propertyName] as IReactiveProperty).Value : (T) _propertyValues[propertyName];
         return default(T);
      }

      /// <summary>
      /// Property mutator.  Use this for observable properties.
      /// </summary>
      /// <typeparam name="T">Property type.</typeparam>
      /// <param name="iValue">Property value.</param>
      /// <param name="propertyName">Property name.</param>
      protected void Set<T>(T iValue, [CallerMemberName] string propertyName = null)
      {
         _propertyValues[propertyName] = iValue;
         Changed(propertyName);
      }

      /// <summary>
      /// Fires property changed event.
      /// </summary>
      /// <typeparam name="T">Property type.</typeparam>
      /// <param name="expression">Expression containing property name, to avoid hardcoding it.</param>
      protected void Changed<T>(Expression<Func<T>> expression)
      {
         Changed(((MemberExpression)expression.Body).Member.Name);
      }

      /// <summary>
      /// Fires property changed event.
      /// </summary>
      /// <param name="propertyName">Property name.</param>
      protected virtual void Changed(string propertyName)
      {
         if (PropertyChanged != null)
         {
            // Use a stack to keep track of things to avoid any circular loop situation that might arise.
            if (_propertyChangedStack.Any(x => x == propertyName))
               return;

            _propertyChangedStack.Push(propertyName);
            try
            {
               PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            finally
            {
               _propertyChangedStack.TryPop(out string prop);
            }
         }
      }
   }
}

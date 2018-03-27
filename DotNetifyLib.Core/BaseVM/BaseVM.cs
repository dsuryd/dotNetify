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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json;

namespace DotNetify
{
   using PropertyDictionary = ConcurrentDictionary<string, object>;

   /// <summary>
   /// Attribute to prevent a property from being sent to the client.
   /// </summary>
   public class IgnoreAttribute : Attribute { }

   /// <summary>
   /// Exception that gets thrown a JSON view model update from the client cannot be resolved.
   /// </summary>
   public class UnresolvedVMUpdateException : Exception
   {
      public string PropertyPath { get; set; }
      public string Value { get; set; }
   }

   /// <summary>
   /// Base class for all DotNetify view models.
   /// </summary>
   public class BaseVM : ObservableObject, IReactiveProperties, IPushUpdates, ISerializer, IDeserializer
   {
      private readonly Queue<PropertyDictionary> _propertyDictionaries = new Queue<PropertyDictionary>(Enumerable.Range(0, 2).Select(_ => new PropertyDictionary()));
      private readonly INotifyPropertyChanged _vmInstance;
      private PropertyDictionary _changedProperties;
      private List<string> _ignoredProperties;

      private static readonly VMSerializer _vmSerializer = new VMSerializer();
      protected ISerializer _serializer = _vmSerializer;
      protected IDeserializer _deserializer = _vmSerializer;

      /// <summary>
      /// Occurs when the view model wants to push updates to the client.
      /// This event is handled by the VMController.
      /// </summary>
      public event EventHandler RequestPushUpdates;

      /// <summary>
      /// Runtime reactive properties.
      /// </summary>
      [Ignore]
      public IList<IReactiveProperty> RuntimeProperties { get; set; } = new List<IReactiveProperty>();

      /// <summary>
      /// Gets properties that have been changed after the last accept command.
      /// </summary>
      [Ignore]
      public ConcurrentDictionary<string, object> ChangedProperties => _changedProperties;

      /// <summary>
      /// Gets a list of ignored properties.
      /// </summary>
      [Ignore]
      public List<string> IgnoredProperties => _ignoredProperties = _ignoredProperties ?? VMTypeInfo.GetProperties()
            .Where(i => i.GetCustomAttribute(typeof(IgnoreAttribute)) != null)
            .Select(j => j.Name)
            .ToList();

      /// <summary>
      /// View model's type information.
      /// </summary>
      protected TypeInfo VMTypeInfo => _vmInstance.GetType().GetTypeInfo();

      /// <summary>
      /// Default constructor.
      /// </summary>
      public BaseVM() : base()
      {
         _vmInstance = this;
         PropertyChanged += OnPropertyChanged;
         _changedProperties = _propertyDictionaries.Dequeue();
      }

      /// <summary>
      /// Constructor to create a wrapper for a view model that doesn't inherit from BaseVM.
      /// </summary>
      /// <param name="vm">View model instance.</param>
      internal BaseVM(INotifyPropertyChanged vm) : base()
      {
         _vmInstance = vm;
         vm.PropertyChanged += OnPropertyChanged;
         _changedProperties = _propertyDictionaries.Dequeue();

         if (vm is IReactiveProperties && (vm as IReactiveProperties).RuntimeProperties != null)
         {
            var runtimeProperties = (vm as IReactiveProperties).RuntimeProperties.Where(prop => !string.IsNullOrWhiteSpace(prop.Name)).ToList();
            runtimeProperties.ForEach(prop =>
            {
               prop.PropertyChanged += OnPropertyChanged;
               RuntimeProperties.Add(prop);
               Set(prop, prop.Name);
            });
            Disposed += (sender, e) => runtimeProperties.ForEach(prop => prop.PropertyChanged -= OnPropertyChanged);
            IgnoredProperties.Add(nameof(IReactiveProperties.RuntimeProperties));
         }

         if (vm is IPushUpdates)
            (vm as IPushUpdates).RequestPushUpdates += (sender, e) => RequestPushUpdates?.Invoke(this, e);

         if (vm is IDisposable)
            Disposed += (sender, e) =>
            {
               vm.PropertyChanged -= OnPropertyChanged;
               (vm as IDisposable).Dispose();
            };

         if (vm is IBaseVMAccessor)
            (vm as IBaseVMAccessor).OnInitialized?.Invoke(this);
      }

      /// <summary>
      /// Creates a view model instance.
      /// </summary>
      /// <param name="registeredTypes">Registered view model types.</param>
      /// <param name="vmTypeName">View model type name.</param>
      /// <param name="vmInstanceId">Optional view model instance identifier.</param>
      /// <param name="vmNamespace">Optional view model type namespace.</param>
      /// <returns></returns>
      internal static BaseVM Create(IEnumerable<TypeHelper> registeredTypes, string vmTypeName, string vmInstanceId = null, string vmNamespace = null)
      {
         TypeHelper vmType = vmNamespace != null ?
            registeredTypes.FirstOrDefault(i => i.FullName == $"{vmNamespace}.{vmTypeName}") :
            registeredTypes.FirstOrDefault(i => i.Name == vmTypeName);

         if (vmType == null)
            return null;

         try
         {
            if (vmInstanceId != null)
            {
               var instance = vmType.CreateInstance(new object[] { vmInstanceId }) as INotifyPropertyChanged;
               if (instance != null)
                  return instance is BaseVM ? instance as BaseVM : new BaseVM(instance);
            }
         }
         catch (MissingMethodException)
         {
            Trace.Fail($"[dotNetify] ERROR: '{vmTypeName}' has no constructor accepting instance ID.");
         }

         try
         {
            var instance = vmType.CreateInstance(null) as INotifyPropertyChanged;
            if (instance != null)
               return instance is BaseVM ? instance as BaseVM : new BaseVM(instance);
         }
         catch (MissingMethodException)
         {
            Trace.Fail($"[dotNetify] ERROR: '{vmTypeName}' has no parameterless constructor.");
         }

         return null;
      }

      /// <summary>
      /// Accepts all changed properties so they won't marked as changed anymore.
      /// </summary>
      /// <returns>Accepted changed properties.</returns>
      internal IDictionary<string, object> AcceptChangedProperties()
      {
         IDictionary<string, object> result = null;
         lock (_propertyDictionaries)
         {
            var changedProperties = Interlocked.Exchange(ref _changedProperties, _propertyDictionaries.Dequeue());
            if (changedProperties.Count > 0)
               result = new Dictionary<string, object>(changedProperties);
            changedProperties.Clear();
            _propertyDictionaries.Enqueue(changedProperties);
         }
         return result;
      }

      /// <summary>
      /// Adds a runtime reactive property.
      /// </summary>
      /// <param name="vm">View model to add the property to.</param>
      /// <param name="propertyName">Property name.</param>
      /// <param name="propertyValue">Property value.</param>
      /// <returns>Reactive property.</returns>
      public ReactiveProperty<T> AddProperty<T>(string propertyName) => AddProperty(typeof(T), new ReactiveProperty<T>(propertyName));

      /// <summary>
      /// Adds a runtime reactive property.
      /// </summary>
      /// <param name="vm">View model to add the property to.</param>
      /// <param name="propertyName">Property name.</param>
      /// <param name="propertyValue">Property value.</param>
      /// <returns>Reactive property.</returns>
      public ReactiveProperty<T> AddProperty<T>(string propertyName, T propertyValue) => AddProperty(typeof(T), new ReactiveProperty<T>(propertyName, propertyValue));

      /// <summary>
      /// Adds a runtime observable property.
      /// </summary>
      /// <param name="propertyName">Property name.</param>
      private ReactiveProperty<T> AddProperty<T>(Type propertyType, ReactiveProperty<T> property)
      {
         if (_propertyValues.ContainsKey(property.Name))
            throw new InvalidOperationException($"{property.Name} already exists.");

         property.Subscribe(_ => Changed(property.Name));
         RuntimeProperties.Add(property);
         Set(property, property.Name);
         return property;
      }

      /// <summary>
      /// Adds a runtime reactive property for internal server-side use.
      /// </summary>
      /// <param name="vm">View model to add the property to.</param>
      /// <param name="propertyName">Property name.</param>
      /// <param name="propertyValue">Property value.</param>
      /// <returns>Reactive property.</returns>
      public ReactiveProperty<T> AddInternalProperty<T>(string propertyName)
      {
         Ignore(propertyName);
         return AddProperty(typeof(T), new ReactiveProperty<T>(propertyName));
      }

      /// <summary>
      /// Adds a runtime reactive property for internal server-side use.
      /// </summary>
      /// <param name="vm">View model to add the property to.</param>
      /// <param name="propertyName">Property name.</param>
      /// <param name="propertyValue">Property value.</param>
      /// <returns>Reactive property.</returns>
      public ReactiveProperty<T> AddInternalProperty<T>(string propertyName, T propertyValue)
      {
         Ignore(propertyName);
         return AddProperty(typeof(T), new ReactiveProperty<T>(propertyName, propertyValue));
      }

      /// <summary>
      /// Override this method if the derived type is a master view model.  The VMController
      /// will call this method to get instances of any view model whose view falls within
      /// this master view in the HTML markup.  The master view model can use this opportunity
      /// to do its own initialization of those subordinate view models, and/or arranging
      /// communication among them. If null is returned, the VMController will create the
      /// instance itself.
      /// </summary>
      /// <param name="vmTypeName">View model type name.</param>
      /// <param name="vmInstanceId">View model instance identifier.</param>
      /// <param name="iVMArg">View model's initialization argument.</param>
      /// <returns>View model instance.</returns>
      public virtual BaseVM GetSubVM(string vmTypeName, string vmInstanceId)
      {
         if (_vmInstance is IMasterVM)
         {
            var subVM = (_vmInstance as IMasterVM).GetSubVM(vmTypeName, vmInstanceId);
            return subVM is BaseVM ? subVM as BaseVM : subVM != null ? new BaseVM(subVM) : null;
         }

         return string.IsNullOrEmpty(vmInstanceId) ? GetSubVM(vmTypeName) : null;
      }

      /// <summary>
      /// Overload of GetSubVM that only accepts view model type name.
      /// </summary>
      /// <param name="vmTypeName">View model type name.</param>
      /// <returns>View model instance.</returns>
      public virtual BaseVM GetSubVM(string vmTypeName) => null;

      /// <summary>
      /// Prevent a property from being bound.
      /// </summary>
      /// <typeparam name="T">Property type.</typeparam>
      /// <param name="expression">Expression containing property name, to avoid hardcoding it.</param>
      public void Ignore<T>(Expression<Func<T>> expression) => Ignore(((MemberExpression)expression.Body).Member.Name);

      /// <summary>
      /// Prevent a property from being bound.
      /// </summary>
      /// <param name="propertyName">Property name to ignore.</param>
      public void Ignore(string propertyName)
      {
         if (!IgnoredProperties.Contains(propertyName))
            IgnoredProperties.Add(propertyName);
      }

      /// <summary>
      /// Override this method to access new instances of subordinates view models as soon as they're created.
      /// </summary>
      /// <param name="subVM">Sub-view model instance.</param>
      public virtual void OnSubVMCreated(BaseVM subVM) => (_vmInstance as IMasterVM)?.OnSubVMCreated(subVM._vmInstance);

      /// <summary>
      /// Override this method to access instances of subordinates view models before they're disposed.
      /// </summary>
      /// <param name="subVM">Sub-view model instance.</param>
      public virtual void OnSubVMDisposing(BaseVM subVM) => (_vmInstance as IMasterVM)?.OnSubVMDisposing(subVM._vmInstance);

      /// <summary>
      /// Override this method to handle a value update from a property path that cannot
      /// be resolved by the VMController.
      /// </summary>
      /// <param name="vmPath">View model property path.</param>
      /// <param name="value">New value.</param>
      public virtual void OnUnresolvedUpdate(string vmPath, string value)
      {
         if (_vmInstance != this)
            throw new UnresolvedVMUpdateException() { PropertyPath = vmPath, Value = value };
      }

      /// <summary>
      /// Push property changed updates to the client.
      /// </summary>
      public void PushUpdates()
      {
         if (ChangedProperties.Any())
            RequestPushUpdates?.Invoke(this, null);
      }

      /// <summary>
      /// Serializes the instance into JSON-formatted string.
      /// </summary>
      /// <returns>Serialized string.</returns>
      internal string Serialize()
      {
         var serializer = _vmInstance as ISerializer ?? this;
         return serializer.Serialize(_vmInstance, IgnoredProperties);
      }

      /// <summary>
      /// Serializes only changed properties into JSON-formatted string.
      /// </summary>
      /// <returns>Serialized string.</returns>
      internal string SerializeChangedProperties()
      {
         IDictionary<string, object> changedProperties = AcceptChangedProperties();

         var serializer = _vmInstance as ISerializer ?? this;
         return changedProperties != null ? serializer.Serialize(changedProperties, null) : string.Empty;
      }

      /// <summary>
      /// Deserializes a property value of the instance.
      /// </summary>
      /// <param name="vmPath">View model property path.</param>
      /// <param name="newValue">New value.</param>
      internal bool DeserializeProperty(string vmPath, string newValue)
      {
         var deserializer = _vmInstance as IDeserializer ?? this;
         bool success = deserializer.Deserialize(_vmInstance, vmPath, newValue);
         if (success)
         {
            // Don't include the property we just updated in the ChangedProperties of the view model
            // unless the value is changed internally, so that we don't send the same value back to the client
            // during PushUpdates call by this VMController.
            var changedProperties = ChangedProperties;
            if (changedProperties.ContainsKey(vmPath))
            {
               var changedPropertyValue = changedProperties[vmPath] is IReactiveProperty ?
                  (changedProperties[vmPath] as IReactiveProperty).Value : changedProperties[vmPath];

               var changedValue = changedPropertyValue?.ToString() ?? string.Empty;
               bool hasChanged = changedValue != newValue;

               // If property is an array type, use Json serializer to compare the values.
               if (hasChanged && changedPropertyValue?.GetType().IsArray == true)
                  hasChanged = JsonConvert.SerializeObject(changedPropertyValue) != JsonConvert.SerializeObject(JsonConvert.DeserializeObject(newValue));

               if (!hasChanged)
                  changedProperties.TryRemove(vmPath, out object value);
            }
         }
         else
            // If we cannot resolve the property path, forward the info to the instance to give it a chance to resolve it.
            OnUnresolvedUpdate(vmPath, newValue);

         return success;
      }

      /// <summary>
      /// Serializes an object.
      /// </summary>
      /// <param name="instance">Object to serialize.</param>
      /// <param name="ignoredPropertyNames">Names of properties that should not be serialized.</param>
      /// <returns>Serialized string.</returns>
      public virtual string Serialize(object instance, List<string> ignoredPropertyNames) => _serializer.Serialize(instance, ignoredPropertyNames);

      /// <summary>
      /// Deserializes a property value of an object.
      /// </summary>
      /// <param name="instance">Object to deserialize the property to.</param>
      /// <param name="propertyPath">Property path.</param>
      /// <param name="newValue">New value.</param>
      /// <returns>True if the property value was deserialized.</returns>
      public virtual bool Deserialize(object instance, string propertyPath, string newValue) => _deserializer.Deserialize(instance, propertyPath, newValue);

      /// <summary>
      /// Handles property changed event.
      /// </summary>
      private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         // Skip property that's decorated with [Ignore].
         if (IgnoredProperties.Contains(e.PropertyName))
            return;

         // Mark property as changed, to allow the server view model to forward changes back to the client view model.
         _changedProperties[e.PropertyName] = _propertyValues.ContainsKey(e.PropertyName) ? _propertyValues[e.PropertyName] : VMTypeInfo.GetProperty(e.PropertyName)?.GetValue(_vmInstance);
      }
   }
}
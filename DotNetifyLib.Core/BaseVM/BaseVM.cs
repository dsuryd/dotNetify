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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
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

      internal List<Task> AsyncCommands { get; } = new List<Task>();

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
      /// Accepts all changed properties so they won't marked as changed anymore.
      /// </summary>
      /// <returns>Accepted changed properties.</returns>
      internal virtual IDictionary<string, object> AcceptChangedProperties()
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
      public ReactiveProperty<T> AddProperty<T>(string propertyName) => AddProperty(typeof(T), new ReactiveProperty<T>(this, propertyName));

      /// <summary>
      /// Adds a runtime reactive property.
      /// </summary>
      /// <param name="vm">View model to add the property to.</param>
      /// <param name="propertyName">Property name.</param>
      /// <param name="propertyValue">Property value.</param>
      /// <returns>Reactive property.</returns>
      public ReactiveProperty<T> AddProperty<T>(string propertyName, T propertyValue) => AddProperty(typeof(T), new ReactiveProperty<T>(this, propertyName, propertyValue));

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
         return AddProperty(typeof(T), new ReactiveProperty<T>(this, propertyName));
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
         return AddProperty(typeof(T), new ReactiveProperty<T>(this, propertyName, propertyValue));
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
      public void Ignore<T>(Expression<Func<T>> expression) => Ignore(((MemberExpression) expression.Body).Member.Name);

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
      /// Override this method to perform asynchronous initialization of properties after the view model
      /// instance is created but before it is sent to the requesting client.
      /// </summary>
      /// <returns></returns>
      public virtual Task OnCreatedAsync() => Task.CompletedTask;

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
      public virtual void PushUpdates()
      {
         if (ChangedProperties.Any())
            RequestPushUpdates?.Invoke(this, null);
      }

      /// <summary>
      /// Push property changed updates to the client.  It accepts a boolean to force the request be sent to the VMController
      /// even when there is no changed properties in this view model.  The intent is to allow a way for this view model to
      /// push changes in other view models that share the same VMController.
      /// This method also returns a boolean just so it can support fluent chaining.
      /// </summary>
      /// <param name="force">Always send push update request.</param>
      /// <returns>True, just so it returns a value.</returns>
      public virtual bool PushUpdates(bool force)
      {
         if (ChangedProperties.Any() || force)
            RequestPushUpdates?.Invoke(this, null);
         return true;
      }

      /// <summary>
      /// Serializes the instance into JSON-formatted string.
      /// </summary>
      /// <returns>Serialized instance string.</returns>
      internal string Serialize()
      {
         var serializer = _vmInstance as ISerializer ?? this;
         return serializer.Serialize(_vmInstance, IgnoredProperties);
      }

      /// <summary>
      /// Serializes data into JSON-formatted string.
      /// </summary>
      /// <param name="data">Data to be serialized.</param>
      /// <returns>Serialized data string.</returns>
      internal string Serialize(object data)
      {
         var serializer = _vmInstance as ISerializer ?? this;
         return data != null ? serializer.Serialize(data, IgnoredProperties) : null;
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
         if (!success)
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
      /// Checks whether a changed property value is equal to a value.
      /// </summary>
      /// <param name="propPath">Property path of the changed property.</param>
      /// <param name="value">Value to compare with.</param>
      /// <returns>True if the changed property value is equal to the given value.</returns>
      internal bool IsEqualToChangedPropertyValue(string propPath, object value)
      {
         string valueToCompare = Serialize(value);
         if (_changedProperties.ContainsKey(propPath))
         {
            var changedPropertyValue = _changedProperties[propPath] is IReactiveProperty ?
               (_changedProperties[propPath] as IReactiveProperty).Value : _changedProperties[propPath];

            var changedValue = changedPropertyValue?.ToString() ?? string.Empty;
            bool isEqual = changedValue == valueToCompare;

            // If property is an array type, use Json serializer to compare the values.
            if (!isEqual && changedPropertyValue?.GetType().IsArray == true)
               isEqual = JsonConvert.SerializeObject(changedPropertyValue) != valueToCompare;

            return isEqual;
         }
         return false;
      }

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
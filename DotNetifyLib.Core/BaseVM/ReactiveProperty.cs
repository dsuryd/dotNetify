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
using System.ComponentModel;
using System.Reactive.Subjects;

namespace DotNetify
{
   /// <summary>
   /// Defines a view model property that supports Reactive extensions.
   /// </summary>
   public interface IReactiveProperty : INotifyPropertyChanged
   {
      /// <summary>
      /// Property name.
      /// </summary>
      string Name { get; }

      /// <summary>
      /// Property value.
      /// </summary>
      object Value { get; set; }

      /// <summary>
      /// Property type.
      /// </summary>
      Type PropertyType { get; }
   }

   /// <summary>
   /// Provides a view model property that supports Reactive extensions.
   /// </summary>
   public sealed class ReactiveProperty<T> : IReactiveProperty, IObservable<T>, IObserver<T>, IDisposable
   {
      private T _value;
      private readonly SubjectBase<T> _subject;
      private IDisposable _subscription;

      public event PropertyChangedEventHandler PropertyChanged;

      /// <summary>
      /// Property name.
      /// </summary>
      public string Name { get; }

      /// <summary>
      /// Property value.
      /// </summary>
      public object Value
      {
         get => _value;
         set
         {
            _value = (T)value;
            _subject.OnNext(_value);
         }
      }

      /// <summary>
      /// Property type.
      /// </summary>
      public Type PropertyType => typeof(T);

      #region Constructors

      /// <summary>
      /// Default constructor.
      /// </summary>
      public ReactiveProperty()
      {
         _value = default(T);
         _subject = new Subject<T>();
      }

      /// <summary>
      /// Constructor accepting initial property value.
      /// </summary>
      /// <param name="value">Initial value.</param>
      public ReactiveProperty(T value)
      {
         _value = value;
         _subject = new BehaviorSubject<T>(value);
      }

      /// <summary>
      /// Constructor accepting property name.
      /// </summary>
      /// <param name="name">Property name.</param>
      public ReactiveProperty(string name) : this()
      {
         Name = name;
      }

      /// <summary>
      /// Constructor accepting property name and initial property value.
      /// </summary>
      /// <param name="name">Property name.</param>
      /// <param name="value">Initial value.</param>
      public ReactiveProperty(string name, T value) : this(value)
      {
         Name = name;
      }

      #endregion Constructors

      /// <summary>
      /// Disposes the Rx subject.
      /// </summary>
      public void Dispose()
      {
         _subscription?.Dispose();
         _subject.Dispose();
      }

      /// <summary>
      /// Returns a new property object with an initial value.
      /// </summary>
      /// <param name="value">Initial property value.</param>
      public static implicit operator ReactiveProperty<T>(T value) => new ReactiveProperty<T>(value);

      /// <summary>
      /// Returns the property value.
      /// </summary>
      /// <param name="property">Property object.</param>
      public static implicit operator T(ReactiveProperty<T> property) => property._value;

      /// <summary>
      /// Notifies of the end of sequence.
      /// </summary>
      public void OnCompleted() => _subject.OnCompleted();

      /// <summary>
      /// Notifies of an error.
      /// </summary>
      /// <param name="error">Error.</param>
      public void OnError(Exception error) => _subject.OnError(error);

      /// <summary>
      /// Notifies of the next value in the sequence.
      /// </summary>
      /// <param name="value">The next value.</param>
      public void OnNext(T value)
      {
         _value = value;
         _subject.OnNext(value);
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
      }

      /// <summary>
      /// Subscribes an observer to the sequence.
      /// </summary>
      /// <param name="observer">Observer.</param>
      /// <returns>Disposable subscription.</returns>
      public IDisposable Subscribe(IObserver<T> observer) => _subject.Subscribe(observer);

      /// <summary>
      /// Subscribes to an observable.
      /// </summary>
      /// <param name="observable">Observable.</param>
      /// <returns>This object.</returns>
      public ReactiveProperty<T> SubscribeTo(IObservable<T> observable)
      {
         if (_subscription != null)
            throw new InvalidOperationException($"Property '{Name}' is already subscribed to an observable!");

         _subscription = observable.Subscribe(this);
         return this;
      }

      /// <summary>
      /// Unsubscribes current subscription.
      /// </summary>
      public void Unsubscribe()
      {
         _subscription?.Dispose();
         _subscription = null;
      }

      /// <summary>
      /// Returns string representation of the property value.
      /// </summary>
      /// <returns>Property value as string.</returns>
      public override string ToString() => _value.ToString();
   }
}
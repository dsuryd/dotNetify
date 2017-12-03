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

namespace DotNetify
{
   /// <summary>
   /// Provides Type abstraction for both static and runtime view model type.
   /// </summary>
   internal class TypeHelper
   {
      private bool _isRuntimeType;
      private Func<object[], object> _factoryDelegate;

      /// <summary>
      /// Type.
      /// </summary>
      public Type Type { get; }

      /// <summary>
      /// Type name.
      /// </summary>
      public string Name { get; }

      /// <summary>
      /// Full type name.
      /// </summary>
      public string FullName { get; }

      public static implicit operator Type(TypeHelper typeHelper) => typeHelper.Type;
      public static implicit operator TypeHelper(Type type) => new TypeHelper(type);

      public static bool operator ==(TypeHelper lhs, TypeHelper rhs) => IsEqual(lhs, rhs);
      public static bool operator !=(TypeHelper lhs, TypeHelper rhs) => !IsEqual(lhs, rhs);

      public override bool Equals(object obj) => obj is TypeHelper ? IsEqual(this, obj as TypeHelper) : base.Equals(obj);
      public override int GetHashCode() => base.GetHashCode();

      /// <summary>
      /// Constructor that accepts type.
      /// </summary>
      /// <param name="type">Type.</param>
      public TypeHelper(Type type)
      {
         Type = type;
         Name = type.Name;
         FullName = type.FullName;
         _factoryDelegate = args => VMController.CreateInstance(Type, args);
      }

      /// <summary>
      /// Constructor that accepts type name.
      /// </summary>
      /// <param name="typeName">Type name.</param>
      /// <param name="factoryDelegate">Factory delegate to create objects of this type.</param>
      public TypeHelper(string typeName, Func<object[], object> factoryDelegate)
      {
         _isRuntimeType = true;
         _factoryDelegate = factoryDelegate;
         Name = typeName.Contains(".") ? typeName.Split('.').Last() : typeName;
         FullName = typeName;
      }

      /// <summary>
      /// Creates an instance of this type.
      /// </summary>
      /// <param name="args">Constructor arguments.</param>
      /// <returns></returns>
      public object CreateInstance(object[] args = null) => _factoryDelegate?.Invoke(args);

      /// <summary>
      /// Checks equality of two objects.
      /// </summary>
      /// <returns>True if the types are equal.</returns>
      private static bool IsEqual(TypeHelper lhs, TypeHelper rhs)
      {
         if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
            return ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null);
         else if (lhs._isRuntimeType || rhs._isRuntimeType)
            return lhs._isRuntimeType == rhs._isRuntimeType && lhs.Name == rhs.Name;
         else
            return lhs._isRuntimeType == rhs._isRuntimeType && lhs.Type == rhs.Type;
      }
   }
}

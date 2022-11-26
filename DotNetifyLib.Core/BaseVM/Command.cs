/*
Copyright 2018 Dicky Suryadi

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
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;

namespace DotNetify
{
   /// <summary>
   /// Attribute to identify a method as a command to be included in serialization.
   /// </summary>
   [AttributeUsage(AttributeTargets.Method)]
   public class CommandAttribute : Attribute
   { }

   /// <summary>
   /// Defines a command that needs to execute asynchronously.
   /// </summary>
   public interface IAsyncCommand
   {
      Task ExecuteAsync(object parameter);
   }

   /// <summary>
   /// Basic implementation of ICommand.
   /// </summary>
   public class Command : ICommand
   {
      private readonly Action _executeAction;
      private readonly Func<object, bool> _canExecuteAction;

      /// <summary>
      /// Not implemented.
      /// </summary>
      public event EventHandler CanExecuteChanged
      { add { } remove { } }

      /// <summary>
      /// Can execute the command.
      /// </summary>
      public bool CanExecute(object parameter) => _canExecuteAction == null || _canExecuteAction.Invoke(parameter);

      /// <summary>
      /// Executes the command.
      /// </summary>
      /// <param name="parameter">Not used.</param>
      public void Execute(object parameter) => _executeAction?.Invoke();

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="executeAction">Execute action.</param>
      public Command(Action executeAction) => _executeAction = executeAction;

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="executeAction">Execute action.</param>
      /// <param name="canExecuteAction">Can execute action.</param>
      public Command(Action executeAction, Func<object, bool> canExecuteAction) : this(executeAction)
      {
         _canExecuteAction = canExecuteAction;
      }

      /// <summary>
      /// Convert the parameter to the expected type.
      /// </summary>
      /// <param name="parameter">Parameter to convert.</param>
      /// <param name="type">Type to convert to.</param>
      internal static object ConvertParameter(object parameter, Type type)
      {
         if (parameter != null)
         {
            bool isClass = type.GetTypeInfo().IsClass && type != typeof(string);
            bool isTuple = type.Name.Contains(nameof(Tuple));

            if (isClass || isTuple)
               parameter = JsonConvert.DeserializeObject(parameter.ToString(), type);
            else
            {
               var typeConverter = TypeDescriptor.GetConverter(type);
               if (typeConverter != null)
                  parameter = typeConverter.ConvertFromString(parameter.ToString());
            }
         }
         return parameter;
      }
   }

   /// <summary>
   /// Basic implementation of ICommand that accepts parameter.
   /// </summary>
   /// <typeparam name="T">Parameter type.</typeparam>
   public class Command<T> : ICommand, IAsyncCommand
   {
      private readonly Action<T> _executeAction;
      private readonly Func<T, Task> _executeAsyncAction;
      private readonly Func<object, bool> _canExecuteAction;

      /// <summary>
      /// Not implemented.
      /// </summary>
      public event EventHandler CanExecuteChanged
      { add { } remove { } }

      /// <summary>
      /// Can execute the command.
      /// </summary>
      public bool CanExecute(object parameter) => _canExecuteAction == null || _canExecuteAction.Invoke(parameter);

      /// <summary>
      /// Executes the command.
      /// </summary>
      /// <param name="parameter">command parameter.</param>
      public void Execute(object parameter) => _executeAction?.Invoke(ConvertParameter(parameter));

      /// <summary>
      /// Executes asynchronous command.
      /// </summary>
      /// <param name="parameter">command parameter.</param>
      public Task ExecuteAsync(object parameter) => _executeAsyncAction?.Invoke(ConvertParameter(parameter));

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="executeAction">Execute action.</param>
      public Command(Action<T> executeAction) => _executeAction = executeAction;

      public Command(Func<T, Task> executeAsyncAction) => _executeAsyncAction = executeAsyncAction;

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="executeAction">Execute action.</param>
      /// <param name="canExecuteAction">Can execute action.</param>
      public Command(Action<T> executeAction, Func<object, bool> canExecuteAction) : this(executeAction) => _canExecuteAction = canExecuteAction;

      /// <summary>
      /// Convert the parameter to the expected type.
      /// </summary>
      /// <param name="parameter">Parameter to convert.</param>
      private T ConvertParameter(object parameter) => (T) Command.ConvertParameter(parameter, typeof(T));
   }
}
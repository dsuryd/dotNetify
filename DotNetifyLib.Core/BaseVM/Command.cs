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
using System.Windows.Input;
using Newtonsoft.Json;

namespace DotNetify
{
   /// <summary>
   /// Basic implementation of ICommand.
   /// </summary>
   public class Command : ICommand
   {
      private Action _executeAction;
      private Func<object, bool> _canExecuteAction;

      /// <summary>
      /// Not implemented.
      /// </summary>
      public event EventHandler CanExecuteChanged { add { } remove { } }

      /// <summary>
      /// Can execute the command.
      /// </summary>
      public bool CanExecute(object parameter)
      {
         return _canExecuteAction != null ? _canExecuteAction.Invoke(parameter) : true;
      }

      /// <summary>
      /// Executes the command.
      /// </summary>
      /// <param name="parameter">Not used.</param>
      public void Execute(object parameter)
      {
         _executeAction?.Invoke();
      }

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="executeAction">Execute action.</param>
      public Command(Action executeAction)
      {
         _executeAction = executeAction;
      }

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="executeAction">Execute action.</param>
      /// <param name="canExecuteAction">Can execute action.</param>
      public Command(Action executeAction, Func<object, bool> canExecuteAction) : this(executeAction)
      {
         _canExecuteAction = canExecuteAction;
      }
   }

   /// <summary>
   /// Basic implementation of ICommand that accepts parameter.
   /// </summary>
   /// <typeparam name="T">Parameter type.</typeparam>
   public class Command<T> : ICommand
   {
      private Action<T> _executeAction;
      private Func<object, bool> _canExecuteAction;

      /// <summary>
      /// Not implemented.
      /// </summary>
      public event EventHandler CanExecuteChanged { add { } remove { } }

      /// <summary>
      /// Can execute the command.
      /// </summary>
      public bool CanExecute(object parameter)
      {
         return _canExecuteAction != null ? _canExecuteAction.Invoke(parameter) : true;
      }

      /// <summary>
      /// Executes the command.
      /// </summary>
      /// <param name="parameter">command parameter.</param>
      public void Execute(object parameter)
      {
         // Convert the parameter to the expected type.
         if (parameter != null)
         {
            if (typeof(T).GetTypeInfo().IsClass && typeof(T) != typeof(string))
               parameter = JsonConvert.DeserializeObject<T>(parameter.ToString());
            else
            {
               var typeConverter = TypeDescriptor.GetConverter(typeof(T));
               if (typeConverter != null)
                  parameter = typeConverter.ConvertFromString(parameter.ToString());
            }
         }

         _executeAction?.Invoke((T)parameter);
      }

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="executeAction">Execute action.</param>
      public Command(Action<T> executeAction)
      {
         _executeAction = executeAction;
      }

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="executeAction">Execute action.</param>
      /// <param name="canExecuteAction">Can execute action.</param>
      public Command(Action<T> executeAction, Func<object, bool> canExecuteAction) : this(executeAction)
      {
         _canExecuteAction = canExecuteAction;
      }
   }
}
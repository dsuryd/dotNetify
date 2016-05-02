using System;
using System.Windows.Input;

namespace DotNetify
{
   /// <summary>
   /// Basic implementation of ICommand.
   /// </summary>
   public class Command : ICommand
   {
      private Action _executeAction;

      /// <summary>
      /// Not implemented.
      /// </summary>
      public event EventHandler CanExecuteChanged { add { } remove { } }

      /// <summary>
      /// Not implemented.
      /// </summary>
      public bool CanExecute( object parameter )
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Executes the command.
      /// </summary>
      /// <param name="parameter">Not used.</param>
      public void Execute( object parameter )
      {
         _executeAction?.Invoke();
      }

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="executeAction">Execute action.</param>
      public Command( Action executeAction )
      {
         _executeAction = executeAction;
      }
   }

   /// <summary>
   /// Basic implementation of ICommand that accepts parameter.
   /// </summary>
   /// <typeparam name="T">Parameter type.</typeparam>
   public class Command<T> : ICommand
   {
      private Action<T> _executeAction;

      /// <summary>
      /// Not implemented.
      /// </summary>
      public event EventHandler CanExecuteChanged { add { } remove { } }

      /// <summary>
      /// Not implemented.
      /// </summary>
      public bool CanExecute( object parameter )
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Executes the command.
      /// </summary>
      /// <param name="parameter">command parameter.</param>
      public void Execute( object parameter )
      {
         _executeAction?.Invoke((T) parameter);
      }

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="executeAction">Execute action.</param>
      public Command( Action<T> executeAction )
      {
         _executeAction = executeAction;
      }
   }
}

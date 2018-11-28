using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace HelloWorld
{
   public class HelloWorld : Window
   {
      public HelloWorld()
      {
         InitializeComponent();
         Closing += (sender, e) => (DataContext as IDisposable)?.Dispose();
      }

      private void InitializeComponent()
      {
         AvaloniaXamlLoader.Load(this);
      }
   }
}
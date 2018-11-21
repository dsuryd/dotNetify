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
         DataContext = Bootstrap.Resolve<HelloWorldVMProxy>();

         Closing += (sender, e) => (DataContext as IDisposable).Dispose();
      }

      private void InitializeComponent()
      {
         AvaloniaXamlLoader.Load(this);
      }
   }
}
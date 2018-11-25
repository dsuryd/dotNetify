using System;
using System.Windows;

namespace HelloWorld
{
   public partial class HelloWorld : Window
   {
      public HelloWorld()
      {
         DataContext = Bootstrap.Resolve<HelloWorldVMProxy>();
         Closing += (sender, e) => (DataContext as IDisposable)?.Dispose();
         InitializeComponent();
      }
   }
}
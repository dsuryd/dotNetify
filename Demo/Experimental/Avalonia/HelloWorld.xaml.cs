using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace HelloWorld
{
   public class HelloWorld : Window
   {
      public HelloWorld()
      {
         InitializeComponent();
         DataContext = Bootstrap.Resolve<HelloWorldVMProxy>();
      }

      private void InitializeComponent()
      {
         AvaloniaXamlLoader.Load(this);
      }
   }
}
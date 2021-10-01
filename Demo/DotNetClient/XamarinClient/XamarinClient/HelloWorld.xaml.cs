using System;
using Xamarin.Forms;

namespace XamarinClient
{
   public partial class HelloWorld : ContentPage
   {
      public HelloWorld()
      {
         BindingContext = Bootstrap.Resolve<HelloWorldVMProxy>();
         Disappearing += (sender, e) => (BindingContext as IDisposable)?.Dispose();
         InitializeComponent();
      }
   }
}
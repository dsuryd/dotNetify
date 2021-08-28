using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinClient
{
   public partial class App : Application
   {
      public App()
      {
         InitializeComponent();

         MainPage = new HelloWorld();
      }

      protected override void OnStart()
      {
      }

      protected override void OnSleep()
      {
      }

      protected override void OnResume()
      {
      }
   }
}

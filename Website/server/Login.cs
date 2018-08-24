using DotNetify;
using DotNetify.Elements;

namespace WebSite
{
   public class Login : BaseVM
   {
      public Login()
      {
         AddProperty("User", "guest")
            .WithAttribute(new TextFieldAttribute { Label = "User:" });

         AddProperty<string>("Password")
            .WithAttribute(new TextFieldAttribute { Label = "Password:" });
      }
   }
}
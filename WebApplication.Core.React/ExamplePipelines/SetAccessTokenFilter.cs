using System;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace DotNetify
{
   public class SetAccessTokenAttribute : Attribute { }

   public class SetAccessTokenFilter : IVMFilter<SetAccessTokenAttribute>
   {
      public Task Invoke(SetAccessTokenAttribute attr, VMContext vmContext, NextFilterDelegate next)
      {
         var securePageVM = vmContext.Instance as ViewModels.SecurePageVM;
         var accessToken = vmContext.HubContext.PipelineData.ContainsKey("AccessToken") ? vmContext.HubContext.PipelineData["AccessToken"] : null;

         if (securePageVM != null && accessToken != null)
            securePageVM.SetAccessToken(accessToken as SecurityToken);

         return next(vmContext);
      }
   }
}

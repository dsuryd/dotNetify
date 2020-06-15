using System;
using DotNetify.Security;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace DotNetify.DevApp
{
   public class ExtractAccessTokenMiddleware : JwtBearerAuthenticationMiddleware
   {
      public ExtractAccessTokenMiddleware(TokenValidationParameters tokenValidationParameters) : base(tokenValidationParameters)
      {
      }

      public override Task Invoke(DotNetifyHubContext hubContext, NextDelegate next)
      {
         if (hubContext.Headers != null)
         {
            try
            {
               ValidateBearerToken(ParseHeaders<HeaderData>(hubContext.Headers), out SecurityToken validatedToken);
               if (validatedToken != null)
                  hubContext.PipelineData.Add("AccessToken", validatedToken);
            }
            catch (Exception)
            {
            }
         }

         return next(hubContext);
      }
   }
}
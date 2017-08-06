using DotNetify.Security;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace DotNetify
{
   public class ExtractAccessTokenMiddleware : JwtBearerAuthenticationMiddleware
   {
      public ExtractAccessTokenMiddleware(TokenValidationParameters tokenValidationParameters) : base(tokenValidationParameters) { }

      public override Task Invoke(DotNetifyHubContext hubContext, NextDelegate next)
      {
         if (hubContext.CallType == nameof(DotNetifyHub.Request_VM))
         {
            ValidateBearerToken(ParseHeaders<HeaderData>(hubContext.Headers), out SecurityToken validatedToken);
            if (validatedToken != null)
               hubContext.PipelineData.Add("AccessToken", validatedToken);
         }

         return next(hubContext);
      }
   }
}

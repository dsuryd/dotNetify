﻿using DotNetify;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Threading.Tasks;

namespace DevApp.ViewModels
{
   /// <summary>
   /// Custom filter to set JWT access token to any view model that has AccessToken property.
   /// </summary>
   public class SetAccessTokenFilter : IVMFilter<SetAccessTokenAttribute>
   {
      public Task Invoke(SetAccessTokenAttribute attr, VMContext vmContext, NextFilterDelegate next)
      {
         var methodInfo = vmContext.Instance.GetType().GetTypeInfo().GetMethod("SetAccessToken");
         var accessToken = vmContext.HubContext.PipelineData.ContainsKey("AccessToken") ? vmContext.HubContext.PipelineData["AccessToken"] : null;

         if (methodInfo != null && accessToken != null)
            methodInfo.Invoke(vmContext.Instance, new object[] { accessToken as SecurityToken });

         return next(vmContext);
      }
   }
}
/* 
Copyright 2017 Dicky Suryadi

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */

using System;

namespace DotNetify.Security
{
   [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
   public class AuthorizeAttribute : Attribute
   {
      public string ClaimType { get; set; }
      public string ClaimValue { get; set; }
      public string Role { get; set; }

      public AuthorizeAttribute()
      {
      }

      public AuthorizeAttribute(string role)
      {
         Role = role;
      }

      public AuthorizeAttribute(string claimType, string claimValue)
      {
         ClaimType = claimType;
         ClaimValue = claimValue;
      }
   }
}

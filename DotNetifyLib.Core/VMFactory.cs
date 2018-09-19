using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace DotNetify
{
   public class VMFactory
   {
      /// <summary>
      /// Creates a view model instance from a list of registered types.
      /// </summary>
      /// <param name="registeredTypes">Registered view model types.</param>
      /// <param name="vmTypeName">View model type name.</param>
      /// <param name="vmInstanceId">Optional view model instance identifier.</param>
      /// <param name="vmNamespace">Optional view model type namespace.</param>
      /// <returns></returns>
      internal static BaseVM Create(IEnumerable<TypeHelper> registeredTypes, string vmTypeName, string vmInstanceId = null, string vmNamespace = null)
      {
         var vmType = GetVMTypeHelper(registeredTypes, vmTypeName, vmNamespace);
         if (vmType == null)
            return null;

         object[] arg = vmInstanceId != null ? new object[] { vmInstanceId } : null;
         try
         {
            if (vmType.CreateInstance(arg) is INotifyPropertyChanged instance)
               return instance is BaseVM ? instance as BaseVM : new BaseVM(instance);
         }
         catch (MissingMethodException)
         {
            if (arg != null)
               Trace.Fail($"[dotNetify] ERROR: '{vmTypeName}' has no constructor accepting instance ID.");
            else
               Trace.Fail($"[dotNetify] ERROR: '{vmTypeName}' has no parameterless constructor.");
         }

         return null;
      }

      /// <summary>
      /// Returns a helper object that can create instances of a given view model type.
      /// </summary>
      /// <param name="registeredTypes">Registered view model types.</param>
      /// <param name="vmTypeName">View model type name.</param>
      /// <param name="vmNamespace">Optional view model type namespace.</param>
      /// <returns></returns>
      private static TypeHelper GetVMTypeHelper(IEnumerable<TypeHelper> registeredTypes, string vmTypeName, string vmNamespace)
      {
         return vmNamespace != null ?
            registeredTypes.FirstOrDefault(i => i.FullName == $"{vmNamespace}.{vmTypeName}") :
            registeredTypes.FirstOrDefault(i => i.Name == vmTypeName);
      }
   }
}
/* 
Copyright 2015 Dicky Suryadi

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

using System.ComponentModel;

namespace DotNetify
{
   /// <summary>
   /// Allows a non-subtype of BaseVM to be a master view model.
   /// </summary>
   public interface IMasterVM
   {
      /// <summary>
      /// Called by the VMController to get instances of any view model whose view falls within
      /// this master view in the HTML markup.  The master view model can use this opportunity
      /// to do its own initialization of those subordinate view models, and/or arranging 
      /// communication among them. If null is returned, the VMController will create the 
      /// instance itself.
      /// </summary>
      /// <param name="vmTypeName">View model type name.</param>
      /// <param name="vmInstanceId">View model instance identifier.</param>
      /// <param name="iVMArg">View model's initialization argument.</param> 
      /// <returns>View model instance.</returns>
      INotifyPropertyChanged GetSubVM(string vmTypeName, string vmInstanceId);

      /// <summary>
      /// Provides new instances of subordinates view models as soon as they're created.
      /// </summary>
      /// <param name="subVM">Sub-view model instance.</param>
      void OnSubVMCreated(object subVM);

      /// <summary>
      /// Override this method to access instances of subordinates view models before they're disposed.
      /// </summary>
      /// <param name="subVM">Sub-view model instance.</param>
      void OnSubVMDisposing(object subVM);
   }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DotNetify;

namespace UnitTests
{
   public class Response
   {
      private string _connectionId;
      private string _vmId;
      private string _vmData;

      public string VMId => _vmId;
      public JObject VMData => (JObject)JsonConvert.DeserializeObject(_vmData);

      public void Handler(string connectionId, string vmId, string vmData)
      {
         _connectionId = connectionId;
         _vmId = vmId;
         _vmData = vmData;
      }

      public T GetVM<T>() where T : INotifyPropertyChanged => JsonConvert.DeserializeObject<T>(_vmData);
      public T GetVMProperty<T>(string propName) => (T) VMData[propName]?.ToObject(typeof(T));
      public void Reset()
      {
         _vmId = string.Empty;
         _vmData = string.Empty;
      }
   }

   public class MockVMController<TViewModel> where TViewModel : INotifyPropertyChanged
   {
      private readonly Response _response = new Response();
      private readonly string _vmId;
      private readonly VMController _vmController;

      public event EventHandler<string> OnResponse;

      public MockVMController(TViewModel vmInstance = default(TViewModel))
      {
         if (vmInstance?.Equals(default(TViewModel)) == false)
            VMController.CreateInstance = (type, args) => type == typeof(TViewModel) ? vmInstance : Activator.CreateInstance(type, args);
         else
            VMController.CreateInstance = (type, args) => Activator.CreateInstance(type, args);

         VMController.Register<TViewModel>();

         _vmId = typeof(TViewModel).Name;
         _vmController = new VMController((connectionId, vmId, vmData) =>
         {
            _response.Handler(connectionId, vmId, vmData);
            OnResponse?.Invoke(this, vmData);
         });
      }

      public Response RequestVM()
      {
         _response.Reset();
         _vmController.OnRequestVM("conn1", _vmId);
         return _response;
      }

      public Response RequestVM(string vmId, object vmArg = null)
      {
         _response.Reset();
         _vmController.OnRequestVM("conn1", vmId, vmArg);
         return _response;
      }

      public JObject UpdateVM(Dictionary<string, object> update, string vmId = null)
      {
         _response.Reset();
         _vmController.OnUpdateVM("conn1", vmId ?? _vmId, update);
         return _response.VMData;
      }

      public void DisposeVM(string vmId = null)
      {
         _vmController.OnDisposeVM("conn1", vmId ?? _vmId);
      }
   }
}

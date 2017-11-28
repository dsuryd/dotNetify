using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DotNetify;

namespace UnitTests
{
   public class ResponseStub
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
      public T GetVMProperty<T>(string propName) where T : class => VMData[propName].ToObject(typeof(T)) as T;

      public Dictionary<string, object> MockAction(string actionName, string actionValue) => new Dictionary<string, object>() { { actionName, actionValue } };
   }

   public class MockVMController<TViewModel> where TViewModel : INotifyPropertyChanged
   {
      private readonly ResponseStub _response = new ResponseStub();
      private readonly string _vmId;
      private readonly VMController _vmController;

      public event EventHandler<string> OnResponse;

      public MockVMController(TViewModel vmInstance = default(TViewModel))
      {
         if (vmInstance?.Equals(default(TViewModel)) == false)
            VMController.CreateInstance = (type, args) => type == typeof(TViewModel) ? vmInstance : throw new InvalidOperationException();
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

      public TViewModel RequestVM()
      {
         _vmController.OnRequestVM("conn1", _vmId);
         return _response.GetVM<TViewModel>();
      }

      public ResponseStub RequestVMRaw()
      {
         _vmController.OnRequestVM("conn1", _vmId);
         return _response;
      }

      public T RequestVM<T>(string vmId) where T : INotifyPropertyChanged
      {
         _vmController.OnRequestVM("conn1", vmId);
         return _response.GetVM<T>();
      }

      public JObject UpdateVM(Dictionary<string, object> update, string vmId = null)
      {
         _vmController.OnUpdateVM("conn1", vmId ?? _vmId, update);
         return _response.VMData;
      }

      public void DisposeVM(string vmId = null)
      {
         _vmController.OnDisposeVM("conn1", vmId ?? _vmId);
      }
   }
}

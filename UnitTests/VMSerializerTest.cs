using System.Collections.Generic;
using System.Linq;
using DotNetify;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
   [TestClass]
   public class VMSerializerTest
   {
      private class Employee
      {
         public int Id { get; set; }
         public string Name { get; set; }
      }

      private class EmployeeVM : BaseVM
      {
         [ItemKey(nameof(Employee.Id))]
         public List<Employee> Employees { get; set; } = new();

         public int Count { get => Get<int>(); set => Set(value); }
      }

      [TestMethod]
      public void SerializerSettings_UseVMContractResolver_ByDefault()
      {
         Assert.AreSame(VMSerializer.SerializerSettings.ContractResolver.GetType(), typeof(VMContractResolver));
      }

      [TestMethod]
      public void SerializerSettings_CamelCaseEnabled_PropertyNamesUseCamelCase()
      {
         var config = new DotNetifyConfiguration();
         config.CamelCaseSerialization = true;

         var serializer = new VMSerializer();

         var vm = new EmployeeVM();
         vm.AddProperty<string>("ReactiveProp");
         vm.Employees.Add(new Employee { Id = 2, Name = "John" });
         vm.Count = 2;
         vm.UpdateList(nameof(vm.Employees), new Employee { Id = 1, Name = "Bob" });

         var data = serializer.Serialize(vm, new List<string>());

         Assert.IsTrue(data.Contains("employees"));
         Assert.IsTrue(data.Contains("employees_itemKey"));
         Assert.IsTrue(data.Contains("count"));
         Assert.IsTrue(data.Contains("reactiveProp"));

         data = serializer.Serialize(vm.ChangedProperties, new List<string>());

         Assert.IsTrue(data.Contains("employees_update"));
         Assert.IsTrue(data.Contains("id"));
         Assert.IsTrue(data.Contains("name"));
         Assert.IsTrue(data.Contains("count"));

         config.CamelCaseSerialization = false;
      }

      [TestMethod]
      public void SerializerSettings_CamelCaseNotSet_PropertyNamesUsePascalCase()
      {
         var serializer = new VMSerializer();

         var vm = new EmployeeVM();
         vm.AddProperty<string>("ReactiveProp");
         vm.Employees.Add(new Employee { Id = 2, Name = "John" });
         vm.Count = 2;
         vm.UpdateList(nameof(vm.Employees), new Employee { Id = 1, Name = "Bob" });

         var data = serializer.Serialize(vm, new List<string>());

         Assert.IsTrue(data.Contains("Employees"));
         Assert.IsTrue(data.Contains("Employees_itemKey"));
         Assert.IsTrue(data.Contains("Count"));
         Assert.IsTrue(data.Contains("ReactiveProp"));

         data = serializer.Serialize(vm.ChangedProperties, new List<string>());

         Assert.IsTrue(data.Contains("Employees_update"));
         Assert.IsTrue(data.Contains("Id"));
         Assert.IsTrue(data.Contains("Name"));
         Assert.IsTrue(data.Contains("Count"));
      }
   }
}
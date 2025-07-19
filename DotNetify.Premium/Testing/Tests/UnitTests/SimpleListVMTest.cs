﻿using DotNetify.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using DevApp.ViewModels;
using Xunit;

namespace DotNetify.Testing.UnitTests
{
   public class SimpleListVMTest
   {
      private HubEmulator _hubEmulator;
      private List<Employee> _mockData;

      public struct EmployeeInfo
      {
         public int Id { get; set; }
         public string FirstName { get; set; }
         public string LastName { get; set; }
      }

      private struct ClientState
      {
         public List<EmployeeInfo> Employees { get; set; }
      }

      public SimpleListVMTest()
      {
         _mockData = new List<Employee>()
         {
            new Employee { Id = 1, FirstName = "John", LastName = "Doe "},
            new Employee { Id = 2, FirstName = "Jane", LastName = "Doe "}
         };

         var employeeRepositoryStub = Stubber.Create<IEmployeeRepository>()
            .Setup(x => x.GetAll(It.IsAny<int>())).Returns(_mockData)
            .Setup(x => x.Get(It.IsAny<int>())).Returns((int id) => _mockData.FirstOrDefault(x => x.Id == id))
            .Setup(x => x.Update(It.IsAny<Employee>())).Calls((Employee update) =>
            {
               var data = _mockData.First(x => x.Id == update.Id);
               data.FirstName = update.FirstName;
               data.LastName = update.LastName;
            })
            .Setup(x => x.Remove(It.IsAny<int>())).Calls((int id) => _mockData.Remove(_mockData.First(x => x.Id == id)))
            .Object;

         _hubEmulator = new HubEmulatorBuilder()
            .AddServices(services => services.AddTransient(x => employeeRepositoryStub))
            .Register<SimpleListVM>(nameof(SimpleListVM))
            .Build();
      }

      [Fact]
      public void Connect_ReturnsInitialState()
      {
         var response = _hubEmulator
            .CreateClient()
            .Connect(nameof(SimpleListVM))
            .As<ClientState>();

         Assert.Equal(2, response.Employees.Count);
      }

      [Fact]
      public void AddItem_ListAdded()
      {
         var client = _hubEmulator.CreateClient();

         client.Connect(nameof(SimpleListVM));
         client.Dispatch(new { Add = "Babe Ruth" });

         Assert.Contains(client.GetState<ClientState>().Employees, x => x.FirstName == "Babe" && x.LastName == "Ruth");
      }

      [Fact]
      public void UpdateItem_ListUpdated()
      {
         var client = _hubEmulator.CreateClient();
         client.Connect(nameof(SimpleListVM));

         var item = client.GetState<ClientState>().Employees.First();
         client.Dispatch(new { Update = new { item.Id, FirstName = "Johnny", LastName = "Tango" } });

         Assert.Contains(_mockData, x => x.FirstName == "Johnny" && x.LastName == "Tango");
      }

      [Fact]
      public void UpdateItem_ListRemoved()
      {
         var client = _hubEmulator.CreateClient();
         client.Connect(nameof(SimpleListVM));

         var item = client.GetState<ClientState>().Employees.First();
         client.Dispatch(new { Remove = item.Id });

         Assert.Single(client.GetState<ClientState>().Employees);
         Assert.Single(_mockData);
      }
   }
}
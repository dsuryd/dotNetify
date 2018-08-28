using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using DotNetify;
using DotNetify.Elements;
using Newtonsoft.Json;

namespace DotNetify.DevApp
{
   public class SimpleListExample : BaseVM
   {
      public SimpleListExample()
      {
         var markdown = new Markdown("DotNetify.DevApp.Docs.Examples.SimpleList.md");

         AddProperty("ViewSource", markdown.GetSection(null, "SimpleListVM.cs"));
         AddProperty("ViewModelSource", markdown.GetSection("SimpleListVM.cs"));
      }
   }   

   public class SimpleListVM : BaseVM
   {
      private readonly IEmployeeRepository _repository;

      public class EmployeeInfo : ObservableObject
      {
         public int Id { get; set; }
         public string FirstName { get; set; }
         public string LastName { get; set; }
      }

      public IEnumerable<EmployeeInfo> Employees => _repository
         .GetAll().Select(i => new EmployeeInfo { Id = i.Id, FirstName = i.FirstName, LastName = i.LastName });       

      public ICommand Add => new Command<string>(arg =>
      {
         var employeeInfo = (EmployeeInfo)JsonConvert.DeserializeObject(arg, typeof(EmployeeInfo));
         var employee = new Employee { FirstName = employeeInfo.FirstName, LastName = employeeInfo.LastName };
         NewId = _repository.Add(employee);
      });

      public int NewId
      {
         get { return Get<int>(); }
         set { Set(value); }
      }

      public ICommand Remove => new Command<string>(id => _repository.Remove(int.Parse(id)));

      public SimpleListVM(IEmployeeRepository repository)
      {
         _repository = repository;
      }

      /// <summary>
      /// By convention, when dotNetify receives a list item from the client, it will look for the function that starts
      /// with the list property name and ends with "_get" to access the list item for the purpose of updating its value.
      /// </summary>
      public EmployeeInfo Employees_get(string iKey)
      {
         EmployeeInfo employeeInfo = null;

         var record = _repository.Get(int.Parse(iKey));
         if (record != null)
         {
            employeeInfo = new EmployeeInfo { Id = record.Id, FirstName = record.FirstName, LastName = record.LastName };
            if (employeeInfo != null)
               employeeInfo.PropertyChanged += Employee_PropertyChanged;
         }

         return employeeInfo;
      }

      private void Employee_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         var employeeInfo = sender as EmployeeInfo;

         var employee = _repository.Get(employeeInfo.Id);
         if (employee != null)
         {
            employee.FirstName = employeeInfo.FirstName;
            employee.LastName = employeeInfo.LastName;
            _repository.Update(employee);
         }
      }   
   }
}
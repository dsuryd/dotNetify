using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DotNetify.Elements;

namespace DotNetify.DevApp
{
   public class SimpleListExample : BaseVM
   {
      public SimpleListExample()
      {
         var markdown = new Markdown("DotNetify.DevApp.Docs.Examples.SimpleList.md");

         AddProperty("ViewSource", markdown.GetSection(null, "SimpleListVM.cs"))
             .SubscribeTo(AddInternalProperty<string>("Framework").Select(GetViewSource));

         AddProperty("ViewModelSource", markdown.GetSection("SimpleListVM.cs"));
      }

      private string GetViewSource(string framework)
      {
         return framework == "Knockout" ?
            new Markdown("DotNetify.DevApp.Docs.Knockout.Examples.SimpleList.md") :
            framework == "Vue" ?
            new Markdown("DotNetify.DevApp.Docs.Vue.Examples.SimpleList.md") :
            new Markdown("DotNetify.DevApp.Docs.Examples.SimpleList.md").GetSection(null, "SimpleListVM.cs");
      }
   }

   public class SimpleListVM : MulticastVM
   {
      private readonly IEmployeeRepository _repository;
      private readonly IConnectionContext _connectionContext;

      public class EmployeeInfo
      {
         public int Id { get; set; }
         public string FirstName { get; set; }
         public string LastName { get; set; }
      }

      // If you use CRUD methods on a list, you must set the item key prop name of that list with ItemKey attribute.
      [ItemKey(nameof(Employee.Id))]
      public IEnumerable<EmployeeInfo> Employees { get; private set; }

      // Clients from the same IP address will share the same VM instance.
      public override string GroupName => _connectionContext.HttpConnection.RemoteIpAddress.ToString();

      public SimpleListVM(IEmployeeRepository repository, IConnectionContext connectionContext)
      {
         _repository = repository;
         _connectionContext = connectionContext;

         Employees = _repository.GetAll(7)
            .Select(i => new EmployeeInfo { Id = i.Id, FirstName = i.FirstName, LastName = i.LastName });
      }

      [Command] /* only for Knockout client */
      public void Add(string fullName)
      {
         var names = fullName.Split(new char[] { ' ' }, 2);
         var employee = new Employee
         {
            FirstName = names.First(),
            LastName = names.Length > 1 ? names.Last() : ""
         };

         // Use CRUD base method to add the list item on the client.
         this.AddList("Employees", new EmployeeInfo
         {
            Id = _repository.Add(employee),
            FirstName = employee.FirstName,
            LastName = employee.LastName
         });
      }

      [Command] /* only for Knockout client */
      public void Update(EmployeeInfo employeeInfo)
      {
         var employee = _repository.Get(employeeInfo.Id);
         if (employee != null)
         {
            employee.FirstName = employeeInfo.FirstName ?? employee.FirstName;
            employee.LastName = employeeInfo.LastName ?? employee.LastName;
            _repository.Update(employee);

            this.UpdateList(nameof(Employees), new EmployeeInfo
            {
               Id = employee.Id,
               FirstName = employee.FirstName,
               LastName = employee.LastName
            });
         }
      }

      [Command] /* only for Knockout client */
      public void Remove(int id)
      {
         _repository.Remove(id);

         // Use CRUD base method to remove the list item on the client.
         this.RemoveList(nameof(Employees), id);
      }
   }
}
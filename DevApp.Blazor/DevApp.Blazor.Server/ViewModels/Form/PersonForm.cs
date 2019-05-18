using System.Linq;
using System.Reactive.Linq;
using DotNetify;
using DotNetify.Elements;

namespace DevApp.Blazor.Server
{
   public class PersonForm : BaseVM
   {
      public ReactiveProperty<Customer> Customer { get; } = new ReactiveProperty<Customer>();

      public PersonForm()
      {
         AddProperty<string>(nameof(NameInfo.FullName))
            .WithAttribute(new TextFieldAttribute { Label = "Name:" })
            .SubscribeTo(Customer.Select(x => x.Name.FullName));

         AddProperty<NamePrefix>(nameof(NameInfo.Prefix))
            .WithAttribute(new DropdownListAttribute { Label = "Prefix:", Options = typeof(NamePrefix).ToDescriptions() })
            .SubscribeTo(Customer.Select(x => x.Name.Prefix));

         AddProperty<string>(nameof(NameInfo.FirstName))
            .WithAttribute(new TextFieldAttribute { Label = "First Name:", MaxLength = 35 })
            .WithRequiredValidation()
            .SubscribeTo(Customer.Select(x => x.Name.FirstName));

         AddProperty<string>(nameof(NameInfo.MiddleName))
            .WithAttribute(new TextFieldAttribute { Label = "Middle Name:", MaxLength = 35 })
            .SubscribeTo(Customer.Select(x => x.Name.MiddleName));

         AddProperty<string>(nameof(NameInfo.LastName))
            .WithAttribute(new TextFieldAttribute { Label = "Last Name:", MaxLength = 35 })
            .WithRequiredValidation()
            .SubscribeTo(Customer.Select(x => x.Name.LastName));

         AddProperty<NameSuffix>(nameof(NameInfo.Suffix))
            .WithAttribute(new DropdownListAttribute { Label = "Suffix:", Options = typeof(NameSuffix).ToDescriptions() })
            .SubscribeTo(Customer.Select(x => x.Name.Suffix));
      }
   }
}
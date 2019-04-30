using System.Linq;
using System.Reactive.Linq;
using DotNetify;
using DotNetify.Elements;

namespace Blazor.Server
{
   public class AddressForm : BaseVM
   {
      public ReactiveProperty<Customer> Customer { get; } = new ReactiveProperty<Customer>();

      public AddressForm()
      {
         AddProperty<string>(nameof(AddressInfo.Address1))
            .WithAttribute(new TextFieldAttribute { Label = "Address 1:" })
            .SubscribeTo(Customer.Select(x => x.Address.Address1));

         AddProperty<string>(nameof(AddressInfo.Address2))
            .WithAttribute(new TextFieldAttribute { Label = "Address 2:" })
            .SubscribeTo(Customer.Select(x => x.Address.Address2));

         AddProperty<string>(nameof(AddressInfo.City))
            .WithAttribute(new TextFieldAttribute { Label = "City:" })
            .SubscribeTo(Customer.Select(x => x.Address.City));

         AddProperty<State>(nameof(AddressInfo.State))
            .WithAttribute(new DropdownListAttribute
            {
               Label = "State:",
               Options = typeof(State).ToDescriptions()
            })
            .SubscribeTo(Customer.Select(x => x.Address.State));

         AddProperty<string>(nameof(AddressInfo.ZipCode))
            .WithAttribute(new TextFieldAttribute { Label = "Zip Code:" })
            .SubscribeTo(Customer.Select(x => x.Address.ZipCode));
      }
   }
}
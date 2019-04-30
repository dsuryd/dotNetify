using System;
using System.Linq;
using System.Reactive.Linq;
using DotNetify;
using DotNetify.Elements;
using DotNetify.Security;

namespace Blazor.Server
{
   [Authorize]
   public class CustomerForm : BaseVM
   {
      private readonly ICustomerRepository _customerRepository;
      private readonly ReactiveProperty<int> _selectedContact;

      public class Contact
      {
         public int Id { get; set; }
         public string Name { get; set; }
         public string Phone { get; set; }
         public string Address { get; set; }
         public string City { get; set; }
         public string ZipCode { get; set; }
      }

      public CustomerForm(ICustomerRepository customerRepository)
      {
         _customerRepository = customerRepository;

         _selectedContact = AddProperty<int>("SelectedContact");

         AddProperty("Contacts", customerRepository.GetAll().Select(customer => ToContact(customer)))
            .WithItemKey(nameof(Contact.Id))
            .WithAttribute(new DataGridAttribute
            {
               RowKey = nameof(Contact.Id),
               Columns = new DataGridColumn[] {
                  new DataGridColumn(nameof(Contact.Name), "Name") { Sortable = true },
                  new DataGridColumn(nameof(Contact.Phone), "Phone") { Sortable = true },
                  new DataGridColumn(nameof(Contact.Address), "Address") { Sortable = true },
                  new DataGridColumn(nameof(Contact.City), "City") { Sortable = true },
                  new DataGridColumn(nameof(Contact.ZipCode), "ZipCode") { Sortable = true }
                },
               Rows = 5
            }.CanSelect(DataGridAttribute.Selection.Single, _selectedContact));

         AddInternalProperty<CustomerFormData>("Submit")
            .SubscribedBy(AddProperty<bool>("SubmitSuccess"), formData => Save(formData));
      }

      public override void OnSubVMCreated(BaseVM subVM)
      {
         // Have sub-forms with 'Customer' property subscribe to the customer data grid's selection changed event.
         var customerPropInfo = subVM.GetType().GetProperty(nameof(Customer));
         if (typeof(ReactiveProperty<Customer>).IsAssignableFrom(customerPropInfo?.PropertyType))
            _selectedContact.SubscribedBy(
               customerPropInfo.GetValue(subVM) as ReactiveProperty<Customer>,
               id => _customerRepository.Get(id)
            );

         if (subVM is NewCustomerForm)
            (subVM as NewCustomerForm).NewCustomer.Subscribe(customer => UpdateContact(customer));
      }

      private bool Save(CustomerFormData formData)
      {
         var id = (int)_selectedContact.Value;
         var customer = _customerRepository.Update(id, formData);

         this.UpdateList("Contacts", ToContact(customer));
         _selectedContact.Value = id;
         return true;
      }

      private Contact ToContact(Customer customer) => new Contact
      {
         Id = customer.Id,
         Name = customer.Name.FullName,
         Address = customer.Address.StreetAddress,
         City = customer.Address.City,
         ZipCode = customer.Address.ZipCode,
         Phone = customer.Phone.PrimaryNumber
      };

      private void UpdateContact(Customer newCustomer)
      {
         this.AddList("Contacts", ToContact(newCustomer));
         _selectedContact.OnNext(newCustomer.Id);
      }
   }
}
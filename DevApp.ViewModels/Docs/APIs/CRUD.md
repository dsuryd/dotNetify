## CRUD

CRUD are such a common task that the APIs to help you with it is baked in the **BaseVM**. Let's say your app has a view that displays a list of items, therefore your view model would have a property that returns that list:

```csharp
public class ContactList : BaseVM
{
   private readonly IContactService _contactService;
   public class Contact
   {
      public int Id { get; set; }
      public string Name { get; set; }
      public string Email { get; set; }
   }
   public List<Contact> Contacts => _contactService.GetAllContacts();

   public ContactList( IContactService contactService )
   {
      _contactService = contactService;
   }
}
```

When you need to add, update or remove an item on the list from the back-end, you can use the following APIs:

- this.**AddList**(_propertyName, item_)
- this.**UpdateList**(_propertyName, item_)
- this.**RemoveList**(_propertyName, itemKey_)

But using any of this API will require you decorate the list property with an **[ItemKey]** attribute to specify the item key of the list. For example:

```csharp
   [ItemKey(nameof(Contact.Id))]
   public List<Contact> Contacts => _contactService.GetAllContacts();

   public void Add(Contact contact) => this.AddList(nameof(Contacts), contact);
   public void Update(Contact contact) => this.UpdateList(nameof(Contacts), contact);
   public void Remove(Contact contact) => this.RemoveList(nameof(Contacts), contact.Id);
```

Note that any of these APIs will merely mark the list property as changed. The update will be sent only if it's part of the action-reaction cycle, or if you initiate the push yourself by calling **PushUpdates**. Once it gets to the front-end, the change will be made to the associated React component's state, which will then cause the list to be re-rendered.

##### SimpleList.html

```html
<div data-vm="SimpleListVM">
  <header>
    <input type="text" class="form-control" placeholder="First name" data-bind="value: _firstName" />
    <input type="text" class="form-control" placeholder="Last name" data-bind="value: _lastName" />
    <button type="button" class="btn btn-primary" data-bind="vmCommand: add">Add</button>
  </header>
  <table>
    <thead>
      <tr>
        <th>First Name</th>
        <th>Last Name</th>
        <th></th>
      </tr>
    </thead>
    <tbody data-bind="foreach: Employees, vmItemKey: Id">
      <tr>
        <td>
          <div>
            <span class="editable" data-bind="text: FirstName, vmCommand: edit"></span>
            <input style="display:none" type="text" data-bind="value: FirstName" />
          </div>
        </td>
        <td>
          <div>
            <span class="editable" data-bind="text: LastName, vmCommand: edit"></span>
            <input style="display:none" type="text" data-bind="value: LastName" />
          </div>

        </td>
        <td>
          <div data-bind="vmCommand: remove">
            <i class="material-icons">clear</i>
          </div>
        </td>
      </tr>
    </tbody>
  </table>
</div>
```

##### SimpleListVM.ts

```jsx
declare var $: any;

export default class SimpleListVM {
  _firstName: string = "";
  _lastName: string = "";

  add() {
    const vm: any = this;

    let person: string = `${vm._firstName()} ${vm._lastName()}`;
    if (person.trim() === "") return;

    vm.Add(person);

    vm._firstName('');
    vm._lastName('');
  }

  edit(item, iElement) {
    // jQuery chain to hide the span element and show the input element;
    // on input element losing focus, do the reverse.
    $(iElement)
      .hide()
      .next()
      .show()
      .focus()
      .one("focusout", function() {
        $(this)
          .hide()
          .prev()
          .show();
      });
  }

  remove(item) {
    var vm: any = this;
    vm.$removeList(vm.Employees, i => i.Id() == item.Id());
    vm.Remove(item.Id());
  }
}
```

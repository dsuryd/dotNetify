import $ from 'jquery';

export default class SimpleListVM {
  _firstName: string = '';
  _lastName: string = '';

  add() {
    var vm: any = this;
    var person: any = { FirstName: vm._firstName(), LastName: vm._lastName() };
    if (!person.FirstName || !person.LastName) return;

    vm.Add(person);

    vm.$once(vm.NewId, iNewId => {
      person.Id = iNewId;
      vm.$addList(vm.Employees, person);

      vm._firstName('');
      vm._lastName('');
    });
  }

  edit(iItem, iElement) {
    // jQuery chain to hide the span element and show the input element;
    // on input element losing focus, do the reverse.
    $(iElement).hide().next().show().focus().one('focusout', function() {
      $(this).hide().prev().show();
    });
  }

  remove(iItem) {
    var vm: any = this;
    vm.$removeList(vm.Employees, i => i.Id() == iItem.Id());
    vm.Remove(iItem.Id());
  }
}

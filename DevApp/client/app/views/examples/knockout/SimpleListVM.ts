declare var $: any;

export default class SimpleListVM {
  _firstName: string = "";
  _lastName: string = "";

  add() {
    const vm: any = this;

    let person: string = `${vm._firstName()} ${vm._lastName()}`;
    if (person.trim() === "") return;

    vm.Add(person);
  }

  edit(item, iElement) {
    // jQuery chain to hide the span element and show the input element; on input element losing focus, do the reverse.
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

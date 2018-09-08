export default class BookStoreVM {
  constructor(vm: any) {
    vm.onRouteEnter = (path: string, template: any) => {
      template.Target('BookPanel');
      if (path !== '') template.ViewUrl('/examples/Book.html');
    };
  }
}

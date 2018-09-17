export default class BookStoreVM {
  constructor(vm: any) {
    vm.onRouteEnter = (path: string, template: any) => {
      template.Target = 'BookPanel';

      if (path.search('book/') >= 0) template.ViewUrl = '/examples/Book.html';
      else return false;
    };
  }
}

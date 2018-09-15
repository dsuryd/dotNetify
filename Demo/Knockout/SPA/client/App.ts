export default class App {
  constructor(vm: any) {
    vm.onRouteEnter = (path: string, template: any) => (template.Target = 'Content');
  }
}

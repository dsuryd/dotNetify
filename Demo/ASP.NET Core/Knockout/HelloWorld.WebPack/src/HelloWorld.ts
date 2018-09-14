import * as ko from 'knockout';

export default class HelloWorld {
	localTime = ko.observable();

	updateLocalTime() {
		const vm: any = this;
		this.localTime(new Date(vm.ServerTime()));
	}
}

"use strict";
exports.__esModule = true;
var SimpleListVM = (function () {
    function SimpleListVM() {
        this._firstName = '';
        this._lastName = '';
    }
    SimpleListVM.prototype.add = function () {
        var vm = this;
        var person = vm._firstName() + " " + vm._lastName();
        if (person.trim() === '')
            return;
        vm.Add(person);
        vm._firstName('');
        vm._lastName('');
    };
    SimpleListVM.prototype.edit = function (item, iElement) {
        $(iElement).hide().next().show().focus().one('focusout', function () {
            $(this).hide().prev().show();
        });
    };
    SimpleListVM.prototype.remove = function (item) {
        var vm = this;
        vm.$removeList(vm.Employees, function (i) { return i.Id() == item.Id(); });
        vm.Remove(item.Id());
    };
    return SimpleListVM;
}());
exports["default"] = SimpleListVM;

"use strict";
exports.__esModule = true;
var BookStoreVM = (function () {
    function BookStoreVM(vm) {
        vm.onRouteEnter = function (path, template) {
            template.Target = 'BookPanel';
            if (path.search('book/') >= 0)
                template.ViewUrl = '/examples/Book.html';
        };
    }
    return BookStoreVM;
}());
exports["default"] = BookStoreVM;

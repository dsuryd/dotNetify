var BetterListVM = (function () {
   return {

      _firstName: "",
      _lastName: "",

      // Adds a new item.
      add: function () {
         var vm = this;

         // Get the user inputs which were bound to the view model's cache values and construct an object to be sent to the server.
         var person = { FirstName: vm._firstName(), LastName: vm._lastName() };

         // Don't add empty names.
         if (!person.FirstName || !person.LastName)
            return;

         // Must clone the object because the server will only send the Id back, and we don't want it to overwrite
         // the the object value to null because we're going to add the object to the list as soon as we get the Id.
         vm.Add(person);

         // When the Id for the new employee is received, set it to the new item and append the item to the list.
         // The $once only subscribes to event once; after it's triggered, it will unsubscribe itself.
         vm.$once(vm.NewId, function (iNewId) {
            person.Id = iNewId;
            vm.$addList(vm.Employees, person);

            // Reset the input boxes.
            vm._firstName("");
            vm._lastName("");
         });
      },

      // Edit an item.
      edit: function (iItem, iElement) {
         // jQuery chain to hide the span element and show the input element; on input element losing focus, do the reverse.
         $(iElement).hide().next().show().focus().one("focusout", function () { $(this).hide().prev().show(); })
      },

      // Removes an item.
      remove: function (iItem) {
         var vm = this;
         vm.$removeList(vm.Employees, function (i) { return i.Id() == iItem.Id() });
         vm.RemoveId(iItem.Id());
      }
   }
})();
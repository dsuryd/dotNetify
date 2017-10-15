"use strict";

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

var _get = function get(_x, _x2, _x3) { var _again = true; _function: while (_again) { var object = _x, property = _x2, receiver = _x3; _again = false; if (object === null) object = Function.prototype; var desc = Object.getOwnPropertyDescriptor(object, property); if (desc === undefined) { var parent = Object.getPrototypeOf(object); if (parent === null) { return undefined; } else { _x = parent; _x2 = property; _x3 = receiver; _again = true; desc = parent = undefined; continue _function; } } else if ("value" in desc) { return desc.value; } else { var getter = desc.get; if (getter === undefined) { return undefined; } return getter.call(receiver); } } };

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var RouteLink = dotnetify.react.router.RouteLink;

var BookStore = (function (_React$Component) {
   _inherits(BookStore, _React$Component);

   function BookStore(props) {
      _classCallCheck(this, BookStore);

      _get(Object.getPrototypeOf(BookStore.prototype), "constructor", this).call(this, props);

      this.vm = dotnetify.react.connect("BookStoreVM", this);
      this.vm.onRouteEnter = function (path, template) {
         return template.Target = "BookPanel";
      };

      this.state = window.vmStates.BookStoreVM;
   }

   _createClass(BookStore, [{
      key: "componentWillUnmount",
      value: function componentWillUnmount() {
         this.vm.$destroy();
      }
   }, {
      key: "render",
      value: function render() {
         return React.createElement(
            MuiThemeProvider,
            null,
            React.createElement(
               "div",
               { className: "container-fluid" },
               React.createElement(
                  "div",
                  { className: "header clearfix" },
                  React.createElement(
                     "h3",
                     null,
                     "Example: Book Store"
                  )
               ),
               React.createElement(
                  Paper,
                  { style: { padding: "1em", marginBottom: "1em" } },
                  "Each product here is represented by a unique URL that can be entered into the address bar to go directly to that specific product page."
               ),
               React.createElement(BookStoreFront, { vm: this.vm, books: this.state.Books }),
               React.createElement("div", { id: "BookPanel" })
            )
         );
      }
   }]);

   return BookStore;
})(React.Component);

var BookStoreFront = (function (_React$Component2) {
   _inherits(BookStoreFront, _React$Component2);

   function BookStoreFront(props) {
      _classCallCheck(this, BookStoreFront);

      _get(Object.getPrototypeOf(BookStoreFront.prototype), "constructor", this).call(this, props);
   }

   _createClass(BookStoreFront, [{
      key: "render",
      value: function render() {
         var _this = this;

         if (this.props.books == null) return React.createElement("div", null);

         var books = this.props.books.map(function (book) {
            return React.createElement(
               "div",
               { key: book.Info.Id, className: "book col-md-3" },
               React.createElement(
                  "center",
                  null,
                  React.createElement(
                     RouteLink,
                     { vm: _this.props.vm, route: book.Route },
                     React.createElement("img", { className: "thumbnail", src: book.Info.ImageUrl })
                  ),
                  React.createElement(
                     "strong",
                     null,
                     book.Info.Title
                  ),
                  React.createElement(
                     "div",
                     null,
                     "by ",
                     React.createElement(
                        "span",
                        null,
                        book.Info.Author
                     )
                  ),
                  React.createElement("br", null)
               )
            );
         });
         return React.createElement(
            "div",
            { className: "row" },
            books
         );
      }
   }]);

   return BookStoreFront;
})(React.Component);

var BookDefault = function BookDefault() {
   return React.createElement("div", null);
};

var Book = (function (_React$Component3) {
   _inherits(Book, _React$Component3);

   function Book(props) {
      _classCallCheck(this, Book);

      _get(Object.getPrototypeOf(Book.prototype), "constructor", this).call(this, props);

      this.vm = dotnetify.react.connect("BookDetailsVM", this);
      this.state = { Book: { Title: "", ImageUrl: "", Author: "", ItemUrl: "" }, open: true };
   }

   _createClass(Book, [{
      key: "componentWillUnmount",
      value: function componentWillUnmount() {
         this.vm.$destroy();
      }
   }, {
      key: "render",
      value: function render() {
         var _this2 = this;

         var book = this.state.Book;

         var handleClose = function handleClose() {
            _this2.setState({ open: false });
            window.history.back();
         };

         var actions = [React.createElement(FlatButton, { label: "Back", primary: true, onClick: handleClose })];

         return React.createElement(
            MuiThemeProvider,
            null,
            React.createElement(
               Dialog,
               { open: this.state.open, actions: actions },
               React.createElement(
                  "div",
                  { className: "row", style: { minHeight: "380px" } },
                  React.createElement(
                     "div",
                     { className: "col-md-4" },
                     React.createElement("img", { className: "thumbnail", src: book.ImageUrl })
                  ),
                  React.createElement(
                     "div",
                     { className: "col-md-8" },
                     React.createElement(
                        "h3",
                        null,
                        book.Title
                     ),
                     React.createElement(
                        "h5",
                        null,
                        book.Author
                     ),
                     React.createElement("br", null),
                     React.createElement(
                        RaisedButton,
                        { primary: true },
                        "Buy"
                     )
                  )
               )
            )
         );
      }
   }]);

   return Book;
})(React.Component);


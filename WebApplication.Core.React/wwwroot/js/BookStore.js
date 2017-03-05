"use strict";

var BookStore = React.createClass({
   displayName: "BookStore",

   getInitialState: function getInitialState() {
      this.vm = dotnetify.react.connect("BookStoreVM", this);
      this.vm.$setRouteTarget("BookPanel");
      return window.vmStates.BookStoreVM;
   },
   componentWillUnmount: function componentWillUnmount() {
      this.vm.$destroy();
      ReactDOM.unmountComponentAtNode(document.getElementById("BookPanel"));
   },
   render: function render() {
      return React.createElement(
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
         React.createElement("div", { id: "BookPanel" })
      );
   }
});

var BookStoreFront = React.createClass({
   displayName: "BookStoreFront",

   getInitialState: function getInitialState() {
      this.vm = dotnetify.react.connect("BookStoreFrontVM", this);
      return { Books: [] };
   },
   componentWillUnmount: function componentWillUnmount() {
      this.vm.$destroy();
   },
   render: function render() {
      var _this = this;

      var books = this.state.Books.map(function (book) {
         return React.createElement(
            "div",
            { key: book.Info.Id, className: "book col-md-3" },
            React.createElement(
               "center",
               null,
               React.createElement(
                  "a",
                  { href: _this.vm.$route(book.Route), onClick: function (event) {
                        return _this.vm.$handleRoute(event);
                     } },
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
});

var Book = React.createClass({
   displayName: "Book",

   getInitialState: function getInitialState() {
      this.vm = dotnetify.react.connect("BookDetailsVM", this);
      return { Book: {} };
   },
   componentWillUnmount: function componentWillUnmount() {
      this.vm.$destroy();
   },
   render: function render() {
      var book = this.state.Book;
      if (book === null) return React.createElement("div", null);

      return React.createElement(
         "div",
         null,
         React.createElement(
            "div",
            { className: "page-header" },
            React.createElement(
               "h3",
               null,
               book.Title
            )
         ),
         React.createElement(
            "div",
            { className: "container" },
            React.createElement(
               "div",
               { className: "row" },
               React.createElement(
                  "div",
                  { className: "col-md-4" },
                  React.createElement("img", { src: book.ImageUrl })
               ),
               React.createElement(
                  "div",
                  { className: "col-md-8" },
                  React.createElement(
                     "div",
                     { className: "page-header" },
                     React.createElement(
                        "h2",
                        null,
                        book.Title
                     )
                  ),
                  React.createElement(
                     "h3",
                     null,
                     book.Author
                  ),
                  React.createElement(
                     "a",
                     { href: book.ItemUrl },
                     React.createElement(
                        "button",
                        { className: "btn btn-lg btn-default" },
                        "Buy"
                     )
                  )
               )
            )
         )
      );
   }
});


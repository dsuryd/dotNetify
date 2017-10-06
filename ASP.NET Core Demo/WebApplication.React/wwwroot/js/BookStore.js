"use strict";

var RouteLink = dotnetify.react.router.RouteLink;

var BookStore = React.createClass({
   displayName: "BookStore",

   getInitialState: function getInitialState() {
      this.vm = dotnetify.react.connect("BookStoreVM", this);
      this.vm.onRouteEnter = function (path, template) {
         return template.Target = "BookPanel";
      };

      return window.vmStates.BookStoreVM;
   },
   componentWillUnmount: function componentWillUnmount() {
      this.vm.$destroy();
   },
   render: function render() {
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
});

var BookStoreFront = React.createClass({
   displayName: "BookStoreFront",

   render: function render() {
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
});

var BookDefault = function BookDefault(props) {
   return React.createElement("div", null);
};

var Book = React.createClass({
   displayName: "Book",

   getInitialState: function getInitialState() {
      this.vm = dotnetify.react.connect("BookDetailsVM", this);
      return { Book: { Title: "", ImageUrl: "", Author: "", ItemUrl: "" }, open: true };
   },
   componentWillUnmount: function componentWillUnmount() {
      this.vm.$destroy();
   },
   render: function render() {
      var _this2 = this;

      var book = this.state.Book;

      var handleClose = function handleClose() {
         _this2.setState({ open: false });
         window.history.back();
      };

      var actions = [React.createElement(FlatButton, { label: "Back", primary: true, onTouchTap: handleClose })];

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
});


const RouteLink = dotnetify.react.router.RouteLink;

class BookStore extends React.Component {
   constructor(props) {
      super(props);

      this.vm = dotnetify.react.connect("BookStoreVM", this);
      this.vm.onRouteEnter = (path, template) => template.Target = "BookPanel";

      this.state = window.vmStates.BookStoreVM;
   }
   componentWillUnmount() {
      this.vm.$destroy();
   }
   render() {
      return (
         <MuiThemeProvider>
            <div className="container-fluid">
               <div className="header clearfix">
                  <h3>Example: Book Store</h3>
               </div>
               <Paper style={{padding: "1em", marginBottom: "1em"}}>
                  Each product here is represented by a unique URL that can be entered into the address bar to go directly to that specific product page.
               </Paper>

               <BookStoreFront vm={this.vm} books={this.state.Books} />
               <div id="BookPanel" />
            </div>
         </MuiThemeProvider>
      )
   }
}

class BookStoreFront extends React.Component {
   constructor(props) {
      super(props);
   }
   render() {
      if (this.props.books == null)
         return <div></div>

      const books = this.props.books.map(book =>
         <div key={book.Info.Id} className="book col-md-3">
            <center>
               <RouteLink vm={this.props.vm} route={book.Route}>
                  <img className="thumbnail" src={book.Info.ImageUrl} />
               </RouteLink>
               <strong>{book.Info.Title}</strong>
               <div>by <span>{book.Info.Author}</span></div>
               <br />
            </center>
         </div>
       )
      return (
         <div className="row">
            {books}
         </div>
      )
   }
}

const BookDefault = () => <div></div>

class Book extends React.Component {
   constructor(props) {
      super(props);

      this.vm = dotnetify.react.connect("BookDetailsVM", this);
      this.state = { Book: { Title: "", ImageUrl: "", Author: "", ItemUrl: "" }, open: true };
   }
   componentWillUnmount() {
      this.vm.$destroy();
   }
   render() {
      var book = this.state.Book;

      const handleClose = () => {
         this.setState({ open: false });
         window.history.back();
      }

      const actions = [<FlatButton label="Back" primary={true} onClick={handleClose} />]

      return (
         <MuiThemeProvider>
            <Dialog open={this.state.open} actions={actions}>
               <div className="row" style={{minHeight: "380px"}}>
                  <div className="col-md-4">
                     <img className="thumbnail" src={book.ImageUrl} />
                  </div>
                  <div className="col-md-8">
                     <h3>{book.Title}</h3>
                     <h5>{book.Author}</h5>
                     <br/>
                     <RaisedButton primary={true}>Buy</RaisedButton>
                  </div>
               </div>
            </Dialog>
         </MuiThemeProvider>
      )
   }
}
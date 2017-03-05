var BookStore = React.createClass({
   getInitialState() {
      this.vm = dotnetify.react.connect("BookStoreVM", this);
      this.vm.$setRouteTarget("BookPanel");
      return window.vmStates.BookStoreVM;
   },
   componentWillUnmount() {
      this.vm.$destroy();
      ReactDOM.unmountComponentAtNode(document.getElementById("BookPanel"));
   },
   render() {
      return (
         <div className="container-fluid">
            <div className="header clearfix">
               <h3>Example: Book Store</h3>
            </div>
            <div id="BookPanel" />
         </div>
      )
   }
});

var BookStoreFront = React.createClass({
   getInitialState() {
      this.vm = dotnetify.react.connect("BookStoreFrontVM", this);
      return { Books: [] };
   },
   componentWillUnmount() {
      this.vm.$destroy();
   },
   render() {
      const books = this.state.Books.map(book =>
         <div key={book.Info.Id} className="book col-md-3">
            <center>
               <a href={this.vm.$route(book.Route)} onClick={event => this.vm.$handleRoute(event)}>
                  <img className="thumbnail" src={book.Info.ImageUrl} />
               </a>
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
});

var Book = React.createClass({
   getInitialState() {
      this.vm = dotnetify.react.connect("BookDetailsVM", this);
      return { Book: {} };
   },
   componentWillUnmount() {
      this.vm.$destroy();
   },
   render() {
      var book = this.state.Book;
      if (book === null)
         return <div></div>

      return (
         <div>
            <div className="page-header">
               <h3>{book.Title}</h3>
            </div>
            <div className="container">
               <div className="row">
                  <div className="col-md-4">
                     <img src={book.ImageUrl} />
                  </div>
                  <div className="col-md-8">
                     <div className="page-header">
                        <h2>{book.Title}</h2>
                     </div>
                     <h3>{book.Author}</h3>
                     <a href={book.ItemUrl}><button className="btn btn-lg btn-default">Buy</button></a>
                  </div>
               </div>
            </div>
         </div>
      )
   }
});
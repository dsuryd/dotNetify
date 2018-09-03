##### BookStore.js

```jsx
import React from 'react';
import dotnetify from 'dotnetify';
import { Modal, Theme } from 'dotnetify-elements';
import { BookStoreCss, BookCss } from './components/css';

const RouteLink = dotnetify.react.router.RouteLink;

class BookStore extends React.Component {
  constructor(props) {
    super(props);
    this.state = {};

    this.vm = dotnetify.react.connect('BookStoreVM', this);
    this.vm.onRouteEnter = (path, template) => (template.Target = 'BookPanel');
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
    return (
      <BookStoreCss>
        <header>
          Each product here is represented by a unique URL that can be entered into the address bar to go directly to
          that specific product page.
        </header>
        <BookStoreFront vm={this.vm} books={this.state.Books} />
        <div id="BookPanel" />
      </BookStoreCss>
    );
  }
}
```

##### BookStoreFront.js

```jsx
const BookStoreFront = ({ vm, books }) => {
  if (!books) return <div />;
  return (
    <section>
      {books.map(book => (
        <div key={book.Info.Id}>
          <center>
            <RouteLink vm={vm} route={book.Route}>
              <img className="thumbnail" src={book.Info.ImageUrl} />
            </RouteLink>
            <div>
              <b>{book.Info.Title}</b>
              <div>{'by ' + book.Info.Author}</div>
            </div>
          </center>
        </div>
      ))}
    </section>
  );
};
```

##### Book.js

```jsx
class Book extends React.Component {
  constructor(props) {
    super(props);
    this.vm = dotnetify.react.connect('BookDetailsVM', this);
    this.state = { Book: { Title: '', ImageUrl: '', Author: '', ItemUrl: '' }, open: true };
  }

  componentWillUnmount() {
    this.vm && this.vm.$destroy();
  }

  handleClose = () => {
    this.setState({ open: false });
    this.vm.$destroy();
    this.vm = null;
    window.history.back();
  };

  render() {
    const book = this.state.Book;
    return (
      <Theme>
        <Modal open={this.state.open}>
          <BookCss>
            <img className="thumbnail" src={book.ImageUrl} />
            <div>
              <h3>{book.Title}</h3>
              <h5>{book.Author}</h5>
              <button className="btn btn-primary">Buy</button>
            </div>
          </BookCss>
          <footer>
            <button className="btn btn-success" onClick={this.handleClose}>
              Back
            </button>
          </footer>
        </Modal>
      </Theme>
    );
  }
}

const BookDefault = () => <div />;
```

##### BookStoreVM.cs

```csharp
public class BookStoreVM : BaseVM, IRoutable
{
  private readonly IWebStoreService _webStoreService;

  public RoutingState RoutingState { get; set; }

  public IEnumerable<object> Books => _webStoreService
      .GetAllBooks()
      .Select(i => new { Info = i, Route = this.GetRoute("Book", "book/" + i.UrlSafeTitle) });

  public BookStoreVM(IWebStoreService webStoreService)
  {
      _webStoreService = webStoreService;

      // Register the route templates with RegisterRoutes method extension of the IRoutable.
      this.RegisterRoutes("examples/BookStore", new List<RouteTemplate>
      {
        new RouteTemplate("BookDefault") { UrlPattern = "" },
        new RouteTemplate("Book") { UrlPattern = "book(/:title)" }
      });
  }
}
```

##### BookDetailsVM.cs

```csharp
public class BookDetailsVM : BaseVM, IRoutable
{
  private readonly IWebStoreService _webStoreService;

  public RoutingState RoutingState { get; set; }
  public WebStoreRecord Book { get; set; }

  public BookDetailsVM(IWebStoreService webStoreService)
  {
      _webStoreService = webStoreService;

      this.OnRouted((sender, e) =>
      {
        if (!string.IsNullOrEmpty(e.From))
        {
            // Extract the book title from the route path.
            var bookTitle = e.From.Replace("book/", "");

            Book = _webStoreService.GetBookByTitle(bookTitle);
            Changed(nameof(Book));
        }
      });
  }
}
```
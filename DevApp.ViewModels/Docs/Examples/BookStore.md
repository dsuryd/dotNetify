##### BookStore.js

```jsx
import React from "react";
import { RouteLink } from "dotnetify/react";
import { Modal, Theme } from "dotnetify-elements";
import { BookStoreCss, BookCss } from "../components/css";

export default BookStore = () => {
  const { vm, state } = useConnect(
    "BookStoreVM",
    {},
    {
      onRouteEnter: (path, template) => (template.Target = "BookPanel")
    }
  );

  return (
    <BookStoreCss>
      <header>
        Each product here is represented by a unique URL that can be entered into the address bar to go directly to that specific product
        page.
      </header>
      <BookStoreFront vm={vm} books={state.Books} />
      <div id="BookPanel" />
    </BookStoreCss>
  );
};
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
              <div>{"by " + book.Info.Author}</div>
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
export const Book = () => {
  const { vm, state, setState } = useConnect("BookDetailsVM", {
    Book: { Title: "", ImageUrl: "", Author: "", ItemUrl: "" },
    open: true
  });

  handleClose = () => {
    setState({ open: false });
    vm.$routeTo(state.BookDefaultRoute);
    vm.$destroy();
  };

  const book = state.Book;
  return (
    <Theme>
      <Modal open={state.open}>
        {book != null ? (
          <BookCss>
            <img className="thumbnail" src={book.ImageUrl} />
            <div>
              <h3>{book.Title}</h3>
              <h5>{book.Author}</h5>
              <button className="btn btn-primary">Buy</button>
            </div>
          </BookCss>
        ) : (
          <div>
            Sorry, we couldn't find any book title that matches <b>{state.SearchTitle}</b>.
          </div>
        )}
        <footer>
          <button className="btn btn-success" onClick={handleClose}>
            Back
          </button>
        </footer>
      </Modal>
    </Theme>
  );
};

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

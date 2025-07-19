import React from "react";
import { BrowserRouter as Router, Switch, Route, Link } from "react-router-dom";
import { Echo } from "./Echo";
import { ChatRoom } from "./ChatRoom";

// This site has 3 pages, all of which are rendered
// dynamically in the browser (not server rendered).
//
// Although the page does not ever refresh, notice how
// React Router keeps the URL up to date as you navigate
// through the site. This preserves the browser history,
// making sure things like the back button and bookmarks
// work properly.

export const App = () => (
  <Router>
    <div>
      <ul>
        <li>
          <Link to="/">Home</Link>
        </li>
        <li>
          <Link to="/echo">Echo</Link>
        </li>
        <li>
          <Link to="/chatroom">Chat Room</Link>
        </li>
      </ul>

      <hr />
      <Switch>
        <Route exact path="/">
          <h1>Choose a test profile</h1>
        </Route>
        <Route path="/echo">
          <Echo />
        </Route>
        <Route path="/chatroom">
          <ChatRoom />
        </Route>
      </Switch>
    </div>
  </Router>
);

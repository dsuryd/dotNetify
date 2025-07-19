import * as React from "react";
import * as ReactDOM from "react-dom";
import { App } from "./App";
import { Connections } from "./pages/Connections";

// Import all the routeable views into the global window variable.
Object.assign(window, { Connections });

ReactDOM.render(<App />, document.getElementById("App"));

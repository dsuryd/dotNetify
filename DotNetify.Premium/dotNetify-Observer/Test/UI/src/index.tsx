import * as React from "react";
import * as ReactDOM from "react-dom";
import { App } from "./App";
import dotnetify from "dotnetify";

dotnetify.hubServerUrl = "http://localhost:5100";
dotnetify.debug = true;

ReactDOM.render(<App />, document.getElementById("App"));

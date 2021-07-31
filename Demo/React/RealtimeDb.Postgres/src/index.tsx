import * as React from "react";
import * as ReactDOM from "react-dom";
import dotnetify from "dotnetify";
import { Businesses } from "./Businesses";

dotnetify.debug = true;

ReactDOM.render(<Businesses />, document.getElementById("App"));

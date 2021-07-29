import * as React from "react";
import * as ReactDOM from "react-dom";
import dotnetify from "dotnetify";
import { User } from "./User";
import { Account } from "./Account";

dotnetify.debug = true;

ReactDOM.render(<User />, document.getElementById("App"));

import * as React from "react";
import * as ReactDOM from "react-dom";
import dotnetify from "dotnetify";
import { UserTable } from "./UserTable";

dotnetify.debug = true;

ReactDOM.render(<UserTable />, document.getElementById("App"));

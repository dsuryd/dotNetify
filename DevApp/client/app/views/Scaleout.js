import React from "react";
import { Markdown, withTheme } from "dotnetify-elements";
import Article from "../components/Article";

const Scaleout = props => (
  <Article vm="Scaleout" id="Content">
    <Markdown id="Content" />
  </Article>
);

export default withTheme(Scaleout);

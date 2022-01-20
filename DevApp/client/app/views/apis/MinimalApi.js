import React from "react";
import { Markdown, withTheme } from "dotnetify-elements";
import Article from "../../components/Article";

const MinimalApi = props => (
  <Article vm="MinimalApi" id="Content">
    <Markdown id="Content" />
  </Article>
);

export default withTheme(MinimalApi);
import React from "react";
import { Markdown, withTheme } from "dotnetify-elements";
import Article from "../../components/Article";

const DotNetifyTesting = () => (
  <Article vm="DotNetifyTesting" id="Content">
    <Markdown id="Content" />
  </Article>
);

export default withTheme(DotNetifyTesting);

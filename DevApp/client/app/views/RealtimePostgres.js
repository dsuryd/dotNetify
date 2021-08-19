import React from "react";
import { Markdown, withTheme } from "dotnetify-elements";
import Article from "../components/Article";

const Image = styled.img`
  display: flex;
  align-items: center;
  justify-content: center;
  max-width: 800px;
  width: 90%;
`;

const RealtimePostgres = props => (
  <Article vm="RealtimePostgres" id="Content">
    <Markdown id="Content" />
  </Article>
);

export default withTheme(RealtimePostgres);

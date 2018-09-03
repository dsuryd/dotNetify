import React from 'react';
import { Markdown, withTheme } from 'dotnetify-elements';
import Article from '../../components/Article';

const Connection = props => (
  <Article vm="Connection" id="Content">
    <Markdown id="Content" />
  </Article>
);

export default withTheme(Connection);

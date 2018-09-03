import React from 'react';
import { Markdown, withTheme } from 'dotnetify-elements';
import Article from '../../components/Article';

const Middleware = props => (
  <Article vm="Middleware" id="Content">
    <Markdown id="Content" />
  </Article>
);

export default withTheme(Middleware);

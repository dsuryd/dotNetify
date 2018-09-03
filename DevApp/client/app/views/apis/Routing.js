import React from 'react';
import { Markdown, withTheme } from 'dotnetify-elements';
import Article from '../../components/Article';

const Routing = props => (
  <Article vm="Routing" id="Content">
    <Markdown id="Content" />
  </Article>
);

export default withTheme(Routing);

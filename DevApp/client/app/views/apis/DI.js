import React from 'react';
import { Markdown, withTheme } from 'dotnetify-elements';
import Article from '../../components/Article';

const DI = props => (
  <Article vm="DI" id="Content">
    <Markdown id="Content" />
  </Article>
);

export default withTheme(DI);

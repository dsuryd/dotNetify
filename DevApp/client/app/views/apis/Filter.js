import React from 'react';
import { Markdown, withTheme } from 'dotnetify-elements';
import Article from '../../components/Article';

const Filter = props => (
  <Article vm="Filter" id="Content">
    <Markdown id="Content" />
  </Article>
);

export default withTheme(Filter);

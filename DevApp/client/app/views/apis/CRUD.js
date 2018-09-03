import React from 'react';
import { Markdown, withTheme } from 'dotnetify-elements';
import Article from '../../components/Article';

const CRUD = props => (
  <Article vm="CRUD" id="Content">
    <Markdown id="Content" />
  </Article>
);

export default withTheme(CRUD);

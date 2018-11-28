import React from 'react';
import { Markdown, withTheme } from 'dotnetify-elements';
import Article from '../../components/Article';

const DotNetClient = props => (
  <Article vm="DotNetClient" id="Content">
    <Markdown id="Content" />
  </Article>
);

export default withTheme(DotNetClient);

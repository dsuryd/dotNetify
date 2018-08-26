import React from 'react';
import { Markdown, withTheme } from 'dotnetify-elements';
import Article from '../components/Article';

const Overview = props => (
   <Article vm="Overview" id="Content">
      <Markdown id="Content" />
   </Article>
);

export default withTheme(Overview);

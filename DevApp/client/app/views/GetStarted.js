import React from 'react';
import { Markdown, withTheme } from 'dotnetify-elements';
import Article from '../components/Article';

const GetStarted = props => (
  <Article vm="GetStarted" id="Content">
    <Markdown id="Content" />
  </Article>
);

export default withTheme(GetStarted);

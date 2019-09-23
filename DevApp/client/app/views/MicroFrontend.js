import React from 'react';
import { Markdown, withTheme } from 'dotnetify-elements';
import Article from '../components/Article';

const MicroFrontend = props => (
  <Article vm="MicroFrontend" id="Content">
    <Markdown id="Content" />
  </Article>
);

export default withTheme(MicroFrontend);

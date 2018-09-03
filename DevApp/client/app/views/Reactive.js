import React from 'react';
import { Markdown, withTheme } from 'dotnetify-elements';
import Article from '../components/Article';

const Reactive = props => (
  <Article vm="Reactive" id="Content">
    <Markdown id="Content" />
  </Article>
);

export default withTheme(Reactive);

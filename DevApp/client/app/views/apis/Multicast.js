import React from 'react';
import { Markdown, withTheme } from 'dotnetify-elements';
import Article from '../../components/Article';

const Multicast = props => (
  <Article vm="Multicast" id="Content">
    <Markdown id="Content" />
  </Article>
);

export default withTheme(Multicast);

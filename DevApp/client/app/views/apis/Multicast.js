import React from 'react';
import { Markdown, withTheme } from 'dotnetify-elements';
import Article from '../../components/Article';

const Multicast = props => (
  <Article vm="Multicast" id="Content">
    <b style={{ color: 'e2544b' }}>*** This feature will be available in the next Nuget release ***</b>
    <Markdown id="Content" />
  </Article>
);

export default withTheme(Multicast);

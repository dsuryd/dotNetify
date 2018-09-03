import React from 'react';
import { Markdown, withTheme } from 'dotnetify-elements';
import Article from '../../components/Article';

const Security = props => (
  <Article vm="Security" id="Content">
    <Markdown id="Content" />
  </Article>
);

export default withTheme(Security);

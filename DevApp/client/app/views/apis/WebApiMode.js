import React from 'react';
import { Markdown, withTheme } from 'dotnetify-elements';
import Article from '../../components/Article';
import { currentFramework, frameworkSelectEvent } from 'app/components/SelectFramework';

class WebApiMode extends React.Component {
  constructor() {
    super();
    this.state = { framework: currentFramework };
    this.unsubs = frameworkSelectEvent.subscribe(framework => this.setState({ framework: framework }));
  }
  componentWillUnmount() {
    this.unsubs();
  }
  render() {
    const { framework } = this.state;
    return (
      <Article vm="WebApiMode" id="Content">
        <Markdown id="Content" />
      </Article>
    );
  }
}

export default withTheme(WebApiMode);

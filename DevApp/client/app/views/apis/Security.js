import React from 'react';
import { Markdown, withTheme } from 'dotnetify-elements';
import Article from '../../components/Article';
import { currentFramework, frameworkSelectEvent } from 'app/components/SelectFramework';

class Security extends React.Component {
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
    return framework === 'Knockout' ? <SecurityKO /> : framework === 'Vue' ? <SecurityVue /> : <SecurityReact />;
  }
}

const SecurityReact = () => (
  <Article vm="Security" id="Content">
    <Markdown id="Content" />
  </Article>
);

const SecurityKO = () => (
  <Article vm="SecurityKO" id="Content">
    <Markdown id="Content" />
  </Article>
);

const SecurityVue = () => (
  <Article vm="SecurityVue" id="Content">
    <Markdown id="Content" />
  </Article>
);

export default withTheme(Security);

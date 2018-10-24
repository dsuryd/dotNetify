import React from 'react';
import { Markdown, withTheme } from 'dotnetify-elements';
import Article from '../../components/Article';
import { currentFramework, frameworkSelectEvent } from 'app/components/SelectFramework';

class Routing extends React.Component {
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
    return framework === 'Knockout' ? <RoutingKO /> : framework === 'Vue' ? <RoutingVue /> : <RoutingReact />;
  }
}

const RoutingReact = props => (
  <Article vm="Routing" id="Content">
    <Markdown id="Content" />
  </Article>
);

const RoutingKO = props => (
  <Article vm="RoutingKO" id="Content">
    <Markdown id="Content" />
  </Article>
);

const RoutingVue = props => (
  <Article vm="RoutingVue" id="Content">
    <Markdown id="Content" />
  </Article>
);

export default withTheme(Routing);

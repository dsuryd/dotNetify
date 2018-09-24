import React from 'react';
import dotnetify from 'dotnetify';
import RenderExample from 'app/components/RenderExample';
import { currentFramework, frameworkSelectEvent } from 'app/components/SelectFramework';

export default class Example extends React.Component {
  constructor() {
    super();
    this.state = { framework: currentFramework };
    this.unsubs = frameworkSelectEvent.subscribe(framework => this.setState({ framework: framework }));
  }
  componentWillUnmount() {
    this.unsubs();
  }
  render() {
    const example = this.state.framework === 'Knockout' ? this.props.ko : this.props.react;
    if (!example) dotnetify.react.router.pushState({}, null, '/core/overview');

    return (
      <RenderExample vm={this.props.vm} vmOptions={{ vmArg: { Framework: this.state.framework } }} key={this.state.framework}>
        {example}
      </RenderExample>
    );
  }
}

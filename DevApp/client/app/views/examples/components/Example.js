import React from 'react';
import dotnetify from 'dotnetify';
import RenderExample from 'app/components/RenderExample';
import { currentFramework, frameworkSelectEvent } from 'app/components/SelectFramework';

export default class Example extends React.Component {
  constructor() {
    super();
    this.state = { framework: currentFramework };
    this.unsubs = frameworkSelectEvent.subscribe(framework => {
      // Add delay to ensure component is completely removed before mounting new one.
      this.setState({ framework: null });
      setTimeout(() => this.setState({ framework: framework }), 200);
    });
  }
  componentWillUnmount() {
    this.unsubs();
  }
  render() {
    const { framework } = this.state;
    const example = framework === 'Knockout' ? this.props.ko : framework === 'Vue' ? this.props.vue : this.props.react;
    if (!example) dotnetify.react.router.pushState({}, null, '/core/overview');

    if (!framework) return null;
    return (
      <RenderExample vm={this.props.vm} vmOptions={{ vmArg: { Framework: framework } }} key={framework}>
        {example}
      </RenderExample>
    );
  }
}

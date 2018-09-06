import React from 'react';
import RenderExample from 'app/components/RenderExample';
import ControlTypesReact from './react/ControlTypes';
import ControlTypesKo from './knockout/ControlTypes';
import { currentFramework, frameworkSelectEvent } from '../framework';

export default class ControlTypes extends React.Component {
  constructor() {
    super();
    this.state = {framework: currentFramework};
    this.unsubs = frameworkSelectEvent.subscribe(framework => this.setState({ framework: framework }));
  }
  componentWillUnmount() {
    this.unsubs();
  }
  render() {
    const example = this.state.framework === 'Knockout' ? <ControlTypesKo /> : <ControlTypesReact />;
    return (
      <RenderExample
        vm="ControlTypesExample"
        vmOptions={{ vmArg: { Framework: this.state.framework } }}
        key={this.state.framework}
      >
        {example}
      </RenderExample>
    );
  }
}

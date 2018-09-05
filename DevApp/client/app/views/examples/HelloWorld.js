import React from 'react';
import RenderExample from 'app/components/RenderExample';
import HelloWorldReact from './react/HelloWorld';
import HelloWorldKo from './knockout/HelloWorld';
import { frameworkSelectEvent } from '../App';

export default class HelloWorld extends React.Component {
  constructor() {
    super();
    this.state = {};
    this.subs = frameworkSelectEvent.subscribe(framework => this.setState({ framework: framework }));
  }
  render() {
    const example = this.state.framework === 'Knockout' ? <HelloWorldKo /> : <HelloWorldReact />;
    return (
      <RenderExample
        vm="HelloWorldExample"
        vmOptions={{ vmArg: { Framework: this.state.framework } }}
        key={this.state.framework}
      >
        {example}
      </RenderExample>
    );
  }
}

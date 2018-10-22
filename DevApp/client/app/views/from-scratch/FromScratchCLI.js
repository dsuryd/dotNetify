import React from 'react';
import { Markdown, withTheme } from 'dotnetify-elements';
import Article from 'app/components/Article';
import { currentFramework, frameworkSelectEvent } from 'app/components/SelectFramework';

class FromScratchCLI extends React.Component {
  constructor() {
    super();
    this.state = { framework: currentFramework };
    this.unsubs = frameworkSelectEvent.subscribe(framework => this.setState({ framework: framework }));
  }
  componentWillUnmount() {
    this.unsubs();
  }
  componentDidUpdate() {
    if (this.state.framework !== 'Vue') dotnetify.react.router.pushState({}, null, '/core/overview');
  }
  render() {
    const { framework } = this.state;
    return framework === 'Vue' ? <FromScratchVueCLI /> : null;
  }
}

const FromScratchVueCLI = _ => (
  <Article vm="FromScratchVueCLI" id="Content">
    <Markdown id="Content" />
  </Article>
);

export default withTheme(FromScratchCLI);

import React from 'react';
import { Markdown, withTheme } from 'dotnetify-elements';
import Article from 'app/components/Article';
import { currentFramework, frameworkSelectEvent } from 'app/components/SelectFramework';

class FromScratchScriptTag extends React.Component {
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
    return framework === 'Knockout' ? (
      <FromScratchScriptTagKO />
    ) : framework === 'Vue' ? (
      <FromScratchScriptTagVue />
    ) : (
      <FromScratchScriptTagReact />
    );
  }
}

const FromScratchScriptTagReact = _ => (
  <Article vm="FromScratchScriptTag" id="Content">
    <Markdown id="Content" />
  </Article>
);

const FromScratchScriptTagKO = _ => (
  <Article vm="FromScratchScriptTagKO" id="Content">
    <Markdown id="Content" />
  </Article>
);

const FromScratchScriptTagVue = _ => (
  <Article vm="FromScratchScriptTagVue" id="Content">
    <Markdown id="Content" />
  </Article>
);

export default withTheme(FromScratchScriptTag);

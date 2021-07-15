import React from "react";
import { Alert, Markdown, withTheme } from "dotnetify-elements";
import Article from "app/components/Article";
import GenerateProject from "app/components/GenerateProject";
import { currentFramework, frameworkSelectEvent } from "app/components/SelectFramework";

class FromScratchWebPack extends React.Component {
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
    return framework === "Knockout" ? (
      <FromScratchWebPackKO />
    ) : framework === "Vue" ? (
      <FromScratchWebPackVue />
    ) : (
      <FromScratchWebPackReact />
    );
  }
}

const FromScratchWebPackReact = _ => (
  <Article vm="FromScratchWebPack" id="Content">
    <Markdown id="Content">
      <Alert info css="margin-bottom: 2rem">
        Want to download the code instead?&nbsp;&nbsp;
        <GenerateProject
          caption="Click here to generate a project."
          title="Generate Hello World project"
          sourceUrl="https://github.com/dsuryd/dotNetify"
          sourceDir="Demo/React/HelloWorld.WebPack"
          useAnchor
        />
      </Alert>
    </Markdown>
  </Article>
);

const FromScratchWebPackKO = _ => (
  <Article vm="FromScratchWebPackKO" id="Content">
    <Markdown id="Content" />
  </Article>
);

const FromScratchWebPackVue = _ => (
  <Article vm="FromScratchWebPackVue" id="Content">
    <Markdown id="Content">
      <Alert info css="margin-bottom: 2rem">
        Want to download the code instead?&nbsp;&nbsp;
        <GenerateProject
          caption="Click here to generate a project."
          title="Generate Hello World project"
          sourceUrl="https://github.com/dsuryd/dotNetify"
          sourceDir="Demo/Vue/HelloWorld.WebPack"
          useAnchor
        />
      </Alert>
    </Markdown>
  </Article>
);

export default withTheme(FromScratchWebPack);

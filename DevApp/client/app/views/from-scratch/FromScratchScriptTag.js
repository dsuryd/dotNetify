import React from "react";
import { Alert, Markdown, withTheme } from "dotnetify-elements";
import Article from "app/components/Article";
import GenerateProject from "app/components/GenerateProject";
import { currentFramework, frameworkSelectEvent } from "app/components/SelectFramework";

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
    return framework === "Knockout" ? (
      <FromScratchScriptTagKO />
    ) : framework === "Vue" ? (
      <FromScratchScriptTagVue />
    ) : (
      <FromScratchScriptTagReact />
    );
  }
}

const FromScratchScriptTagReact = _ => (
  <Article vm="FromScratchScriptTag" id="Content">
    <Markdown id="Content">
      <Alert info css="margin-bottom: 2rem">
        Want to download the code instead?&nbsp;&nbsp;
        <GenerateProject
          caption="Click here to generate a project."
          title="Generate Hello World project"
          sourceUrl="https://github.com/dsuryd/dotNetify"
          sourceDir="Demo/React/HelloWorld.ScriptTag"
          useAnchor
        />
      </Alert>
    </Markdown>
  </Article>
);

const FromScratchScriptTagKO = _ => (
  <Article vm="FromScratchScriptTagKO" id="Content">
    <Markdown id="Content" />
  </Article>
);

const FromScratchScriptTagVue = _ => (
  <Article vm="FromScratchScriptTagVue" id="Content">
    <Markdown id="Content">
      <Alert info css="margin-bottom: 2rem">
        Want to download the code instead?&nbsp;&nbsp;
        <GenerateProject
          caption="Click here to generate a project."
          title="Generate Hello World project"
          sourceUrl="https://github.com/dsuryd/dotNetify"
          sourceDir="Demo/Vue/HelloWorld.ScriptTag"
          useAnchor
        />
      </Alert>
    </Markdown>
  </Article>
);

export default withTheme(FromScratchScriptTag);

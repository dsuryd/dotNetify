import React from "react";
import { Alert, Markdown, withTheme } from "dotnetify-elements";
import Article from "app/components/Article";
import GenerateProject from "app/components/GenerateProject";
import { currentFramework, frameworkSelectEvent } from "app/components/SelectFramework";

class FromScratchCRA extends React.Component {
  constructor() {
    super();
    this.state = { framework: currentFramework };
    this.unsubs = frameworkSelectEvent.subscribe(framework => this.setState({ framework: framework }));
  }
  componentWillUnmount() {
    this.unsubs();
  }
  componentDidUpdate() {
    if (this.state.framework !== "React") dotnetify.react.router.pushState({}, null, "/core/overview");
  }
  render() {
    const { framework } = this.state;
    return framework === "React" ? <FromScratchReactCRA /> : null;
  }
}

const FromScratchReactCRA = _ => (
  <Article vm="FromScratchCRA" id="Content">
    <Markdown id="Content">
      <Alert info css="margin-bottom: 2rem">
        Want to download the code instead?&nbsp;&nbsp;
        <GenerateProject
          caption="Click here to generate a project."
          title="Generate Hello World project"
          sourceUrl="https://github.com/dsuryd/dotNetify"
          sourceDir="Demo/React/HelloWorld.CRA"
          useAnchor
        />
      </Alert>
    </Markdown>
  </Article>
);

export default withTheme(FromScratchCRA);

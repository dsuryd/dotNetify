import React from "react";
import { Markdown, withTheme } from "dotnetify-elements";
import Article from "app/components/Article";
import {
  currentFramework,
  frameworkSelectEvent
} from "app/components/SelectFramework";

class FromScratchCRA extends React.Component {
  constructor() {
    super();
    this.state = { framework: currentFramework };
    this.unsubs = frameworkSelectEvent.subscribe(framework =>
      this.setState({ framework: framework })
    );
  }
  componentWillUnmount() {
    this.unsubs();
  }
  componentDidUpdate() {
    if (this.state.framework !== "React")
      dotnetify.react.router.pushState({}, null, "/core/overview");
  }
  render() {
    const { framework } = this.state;
    return framework === "React" ? <FromScratchReactCRA /> : null;
  }
}

const FromScratchReactCRA = _ => (
  <Article vm="FromScratchCRA" id="Content">
    <Markdown id="Content" />
  </Article>
);

export default withTheme(FromScratchCRA);

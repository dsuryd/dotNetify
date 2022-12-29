import React from "react";
import { Markdown, withTheme } from "dotnetify-elements";
import Article from "../components/Article";

export const DotNetifyTesting = withTheme(() => (
  <Article vm="DotNetifyTesting" id="Content">
    <Markdown id="Content" />
  </Article>
));

export const DotNetifyLoadTester = withTheme(() => (
  <Article vm="DotNetifyLoadTester" id="Content">
    <Markdown id="Content" />
  </Article>
));

export const DotNetifyObserver = withTheme(() => (
  <Article vm="DotNetifyObserver" id="Content">
    <Markdown id="Content" />
  </Article>
));

export const DotNetifyResiliencyAddon = withTheme(() => (
  <Article vm="DotNetifyResiliencyAddon" id="Content">
    <Markdown id="Content" />
  </Article>
));

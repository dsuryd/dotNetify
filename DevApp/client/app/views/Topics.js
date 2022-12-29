import React from "react";
import { Markdown, withTheme } from "dotnetify-elements";
import Article from "../components/Article";
import MfeImage from "../images/MicroFrontend.svg";

const Image = styled.img`
  display: flex;
  align-items: center;
  justify-content: center;
  max-width: 800px;
  width: 90%;
`;

export const MicroFrontend = withTheme(() => (
  <Article vm="MicroFrontend" id="Content">
    <Markdown id="Content">
      <Image src={MfeImage} />
    </Markdown>
  </Article>
));

export const Reactive = withTheme(() => (
  <Article vm="Reactive" id="Content">
    <Markdown id="Content" />
  </Article>
));

export const RealtimePostgres = withTheme(() => (
  <Article vm="RealtimePostgres" id="Content">
    <Markdown id="Content" />
  </Article>
));

export const Scaleout = withTheme(() => (
  <Article vm="Scaleout" id="Content">
    <Markdown id="Content" />
  </Article>
));

export const AWSIntegration = withTheme(() => (
  <Article vm="AWSIntegration" id="Content">
    <Markdown id="Content" />
  </Article>
));

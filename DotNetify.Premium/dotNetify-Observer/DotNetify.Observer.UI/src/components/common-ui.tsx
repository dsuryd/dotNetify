import React from "react";
import { lightTheme } from "dotnetify-elements/components/basic-bundle";
import styled from "styled-components";

export {
  Main,
  Header,
  Nav,
  NavMenu,
  NavMenuTarget,
  NavDrawerButton,
  Panel,
  Section,
  VMContext
} from "dotnetify-elements/components/basic-bundle";

export const theme = {
  ...lightTheme,
  Section: `
     background-color: white;
  `
};

export const Title = styled.h1`
  margin: 0;
  font-weight: 500;
  font-size: 1rem;
`;

const OverlayWithText = styled.div`
  position: fixed;
  width: 100%;
  height: 100%;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.3);
  z-index: 1000;
  cursor: pointer;
  > div {
    position: absolute;
    top: 50%;
    left: 50%;
    font-size: large;
    color: black;
    background-color: white;
    padding: 0.5rem 1rem;
    border-radius: 5px;
    transform: translate(-50%, -50%);
    -ms-transform: translate(-50%, -50%);
  }
`;

export const SystemMessage = ({ children }) => (
  <OverlayWithText>
    <div>{children}</div>
  </OverlayWithText>
);

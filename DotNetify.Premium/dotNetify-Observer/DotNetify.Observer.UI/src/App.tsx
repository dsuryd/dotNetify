import React, { useState } from "react";
import {
  Main,
  Header,
  Nav,
  NavMenu,
  NavMenuTarget,
  NavDrawerButton,
  Panel,
  Section,
  SystemMessage,
  VMContext,
  theme
} from "./components/common-ui";
import { DotNetifyLogo } from "./components/Logo";
import dotnetify from "dotnetify";

dotnetify.debug = true;

export const App = _ => {
  const [connectionState, setConnectionState] = useState<string>();

  dotnetify.connectionStateHandler = state => {
    if (state == "connected") setConnectionState(null);
    else if (state === "connecting") setConnectionState("Connecting to server...");
    else if (state === "reconnecting") setConnectionState("Lost connection to server. Reconnecting...");
    else setConnectionState("Lost connection to server.");
  };

  return (
    <VMContext vm="ObserverAppVM">
      <Main theme={theme}>
        <Header>
          <NavDrawerButton show css="margin-left: 1rem" />
          <DotNetifyLogo />
        </Header>
        <Nav>
          <Panel>
            <NavMenu id="NavMenu" />
          </Panel>
        </Nav>
        <Section>
          <NavMenuTarget />
          {connectionState && <SystemMessage>{connectionState}</SystemMessage>}
        </Section>
      </Main>
    </VMContext>
  );
};

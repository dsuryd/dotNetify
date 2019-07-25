import React from 'react';
import styled from 'styled-components';
import { Main, Header, Nav, NavMenu, NavMenuTarget, NavDrawerButton, Button, Panel, Section, VMContext } from 'dotnetify-elements';

export const Logo = styled.div`
  display: flex;
  align-items: center;
  margin-left: 1rem;
  background-image: url(http://dotnetify.net/content/images/dotnetify-logo.png);
  background-size: 100% 100%;
  width: 200px;
  height: 39px;
`;

const App = _ => (
  <VMContext vm="App">
    <Main>
      <Header>
        <NavDrawerButton show css="margin-left: 1rem" />
        <Logo />
      </Header>
      <Nav>
        <Panel>
          <NavMenu id="NavMenu" />
        </Panel>
      </Nav>
      <Section>
        <NavMenuTarget />
      </Section>
    </Main>
  </VMContext>
);

export default App;

import React from 'react';
import { Main, Header, Nav, NavMenu, NavMenuTarget, NavDrawerButton, Button, Panel, Section, VMContext } from 'dotnetify-elements';
import Logo from '../components/DotNetifyLogo';

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

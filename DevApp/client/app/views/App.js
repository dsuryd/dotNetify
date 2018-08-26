import React from 'react';
import { Main, Header, Footer, Nav, NavMenu, NavMenuTarget, NavDrawerButton, Panel, Section, VMContext } from 'dotnetify-elements';
import Logo, { GitHubLink, TwitterLink, ThemeToggle, LicenseNotice } from '../components/DotNetifyLogo';
import { lightTheme, darkTheme } from 'dotnetify-elements';
import * as utils from '../utils';

const themeToggleEvent = utils.createEventEmitter();

class App extends React.Component {
   constructor(props) {
      super(props);
      this.state = { theme: lightTheme };

      themeToggleEvent.subscribe(arg => {
         this.setState({ theme: arg || this.state.theme.name === 'light' ? darkTheme : lightTheme });
      });
   }

   render() {
      const { theme } = this.state;
      return (
         <VMContext vm="App">
            <Main theme={theme}>
               <Header>
                  <NavDrawerButton show css="margin-left: 1rem" />
                  <Logo />
                  <Panel center middle right padding="1rem">
                     <ThemeToggle name={theme.name} onClick={_ => themeToggleEvent.emit()} />
                     <TwitterLink />
                  </Panel>
               </Header>
               <Nav>
                  <Panel noGap>
                     <GitHubLink />
                     <NavMenu id="NavMenu" />
                  </Panel>
               </Nav>
               <Section>
                  <NavMenuTarget />
               </Section>
               <Footer>
                  <LicenseNotice>
                     © 2015-2018 Dicky Suryadi. Licensed under the{' '}
                     <a href="http://www.apache.org/licenses/LICENSE-2.0">Apache license version 2.0</a>
                  </LicenseNotice>
               </Footer>
            </Main>
         </VMContext>
      );
   }
}

export default App;

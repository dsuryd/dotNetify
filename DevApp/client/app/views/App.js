import React from 'react';
import styled from 'styled-components';
import {
  Main,
  Header,
  Footer,
  Nav,
  NavMenu,
  NavMenuTarget,
  NavDrawerButton,
  Panel,
  Section,
  VMContext
} from 'dotnetify-elements';
import Logo, { GitHubLink, TwitterLink, ThemeToggle, LicenseNotice } from '../components/DotNetifyLogo';
import { lightTheme, darkTheme } from 'dotnetify-elements';
import * as utils from '../utils';
import { currentFramework, frameworkSelectEvent } from './framework';

const themeToggleEvent = utils.createEventEmitter();

const Select = styled.select`
  width: 8rem;
  margin-left: 1rem;
  font-weight: 500;
  background-color: transparent;
  &:focus {
    background-color: transparent;
  }
`;

class App extends React.Component {
  constructor(props) {
    super(props);
    this.state = { theme: lightTheme, framework: currentFramework };

    themeToggleEvent.subscribe(arg => {
      this.setState({ theme: arg || this.state.theme.name === 'light' ? darkTheme : lightTheme });
    });
  }

  handleFrameworkChange = framework => {
    this.setState({ framework: framework });
    frameworkSelectEvent.emit(framework);
  };

  render() {
    const { theme, framework } = this.state;
    console.log(framework);
    return (
      <VMContext vm="App">
        <Main theme={theme}>
          <Header>
            <NavDrawerButton show css="margin-left: 1rem" />
            <Logo />

            <Select
              className="form-control"
              value={framework}
              onChange={e => this.handleFrameworkChange(e.target.value)}
            >
              <option>React</option>
              <option>Knockout</option>
            </Select>

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

export { frameworkSelectEvent, currentFramework };
export default App;

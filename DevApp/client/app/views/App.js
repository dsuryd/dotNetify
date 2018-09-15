import React from 'react';

import { Main, Header, Footer, Nav, NavMenu, NavMenuTarget, NavDrawerButton, Panel, Section, VMContext } from 'dotnetify-elements';
import Logo, { GitHubLink, TwitterLink, ThemeToggle, LicenseNotice } from '../components/DotNetifyLogo';
import SelectFramework, { currentFramework } from '../components/SelectFramework';
import { CoreLink, ElementsLink } from '../components/MenuLinks';
import { lightTheme, darkTheme } from 'dotnetify-elements';
import * as utils from '../utils';

const menuCss = `
  margin-left: 11%;
  @media (max-width: 1170px) {
    margin-left: 5.5rem;
  }   
  @media (max-width: 767px) {
    margin-left: 2rem;
  } 
`;

const themeToggleEvent = utils.createEventEmitter();

class App extends React.Component {
	constructor(props) {
		super(props);
		this.state = { theme: lightTheme, framework: currentFramework };

		themeToggleEvent.subscribe(arg => {
			this.setState({ theme: arg || this.state.theme.name === 'light' ? darkTheme : lightTheme });
		});
	}

	render() {
		const { theme, framework } = this.state;
		return (
			<VMContext vm="App">
				<Main theme={theme}>
					<Header>
						<NavDrawerButton show css="margin-left: 1rem" />
						<Logo />
						<Panel horizontal middle css={menuCss} gap="2rem">
							<CoreLink active={true} />
							<ElementsLink />
						</Panel>
						<Panel center middle right padding="1rem">
							<ThemeToggle name={theme.name} onClick={_ => themeToggleEvent.emit()} />
							<TwitterLink />
						</Panel>
					</Header>
					<Nav>
						<Panel noGap>
							<GitHubLink />
							<SelectFramework id="Framework" onChange={value => this.setState({ framework: value })} />
							<NavMenu id="NavMenu" key={framework} />
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

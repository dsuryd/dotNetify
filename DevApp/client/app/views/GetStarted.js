import React from 'react';
import { Markdown, withTheme } from 'dotnetify-elements';
import Article from '../components/Article';
import FromScratchLink from './from-scratch/FromScratchLink';
import { currentFramework, frameworkSelectEvent } from 'app/components/SelectFramework';

class GetStarted extends React.Component {
	constructor() {
		super();
		this.state = { framework: currentFramework };
		this.unsubs = frameworkSelectEvent.subscribe(framework => this.setState({ framework: framework }));
	}
	componentWillUnmount() {
		this.unsubs();
	}
	render() {
		const { framework } = this.state;
		return framework === 'Knockout' ? <GetStartedKO /> : <GetStartedReact />;
	}
}

const GetStartedReact = props => (
	<Article vm="GetStarted" id="Content">
		<Markdown id="Content">
			<FromScratchLink />
		</Markdown>
	</Article>
);

const GetStartedKO = props => (
	<Article vm="GetStartedKO" id="Content">
		<Markdown id="Content">
			<FromScratchLink />
		</Markdown>
	</Article>
);

export default withTheme(GetStarted);

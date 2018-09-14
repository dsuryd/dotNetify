import React from 'react';
import { Markdown, withTheme } from 'dotnetify-elements';
import Article from 'app/components/Article';
import { currentFramework, frameworkSelectEvent } from 'app/components/SelectFramework';

class FromScratchWebPack extends React.Component {
	constructor() {
		super();
		this.state = { framework: currentFramework };
		this.unsubs = frameworkSelectEvent.subscribe(framework => this.setState({ framework: framework }));
	}
	componentWillUnmount() {
		this.unsubs();
	}
	render() {
		return this.state.framework === 'Knockout' ? <FromScratchWebPackKO /> : <FromScratchWebPackReact />;
	}
}

const FromScratchWebPackReact = _ => (
	<Article vm="FromScratchWebPack" id="Content">
		<Markdown id="Content" />
	</Article>
);

const FromScratchWebPackKO = _ => (
	<Article vm="FromScratchWebPackKO" id="Content">
		<Markdown id="Content" />
	</Article>
);

export default withTheme(FromScratchWebPack);

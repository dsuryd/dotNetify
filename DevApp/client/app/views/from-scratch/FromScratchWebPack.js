import React from 'react';
import dotnetify from 'dotnetify';
import { Markdown, withTheme } from 'dotnetify-elements';
import Article from 'app/components/Article';
import { currentFramework, frameworkSelectEvent } from 'app/components/SelectFramework';

class FromScratchWebPack extends React.Component {
	constructor() {
		super();
		this.state = { framework: currentFramework };
		this.unsubs = frameworkSelectEvent.subscribe((framework) => this.setState({ framework: framework }));
	}
	componentWillUnmount() {
		this.unsubs();
	}
	componentDidUpdate() {
		if (this.state.framework === 'Knockout') dotnetify.react.router.pushState(null, null, '/overview');
	}
	render() {
		return this.state.framework === 'React' ? <FromScratchWebPackReact /> : null;
	}
}

const FromScratchWebPackReact = (_) => (
	<Article vm="FromScratchWebPack" id="Content">
		<Markdown id="Content" />
	</Article>
);

export default withTheme(FromScratchWebPack);

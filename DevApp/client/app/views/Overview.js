import React from 'react';
import dotnetify from 'dotnetify';
import { Markdown, withTheme } from 'dotnetify-elements';
import Article from '../components/Article';
import Expander from '../components/Expander';
import { currentFramework, frameworkSelectEvent } from 'app/components/SelectFramework';

class Overview extends React.Component {
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
		return framework === 'Knockout' ? <OverviewKO /> : <OverviewReact />;
	}
}

const OverviewReact = _ => (
	<Article vm="Overview" id="Content">
		<Markdown id="Content">
			<FromScratchLink />
			<Expander label={<SeeItLive />} content={<RealTimePush />} connectOnExpand />
			<Expander label={<SeeItLive />} content={<ServerUpdate />} />
		</Markdown>
	</Article>
);

const OverviewKO = _ => (
	<Article vm="OverviewKO" id="Content">
		<Markdown id="Content">
			<FromScratchLink framework="Knockout" />
			<Expander label={<SeeItLive />} content={<RealTimePushKO />} connectOnExpand />
			<Expander label={<SeeItLive />} content={<ServerUpdateKO />} />
		</Markdown>
	</Article>
);

const SeeItLive = _ => <b>See It Live!</b>;
const redirect = path => window.dotnetify.react.router.pushState({}, null, path);

const FromScratchLink = ({ framework }) => (
	<ul style={{ fontSize: '1.1rem' }}>
		<li>
			<a href="javascript:void(0)" onClick={_ => redirect('/fromscratch-scripttag')}>
				Real-time "Hello World" from scratch with .NET Core CLI + Script Tag
			</a>
		</li>
		{framework !== 'Knockout' && (
			<li>
				<a href="javascript:void(0)" onClick={_ => redirect('/fromscratch-webpack')}>
					Real-time "Hello World" from scratch with Visual Studio + WebPack
				</a>
			</li>
		)}
	</ul>
);

class RealTimePush extends React.Component {
	constructor(props) {
		super(props);
		this.vm = dotnetify.react.connect('RealTimePush', this);
		this.state = { Greetings: '', ServerTime: '' };
	}
	componentWillUnmount() {
		this.vm.$destroy();
	}
	render() {
		return (
			<div>
				<p>{this.state.Greetings}</p>
				<p>Server time is: {this.state.ServerTime}</p>
			</div>
		);
	}
}

class RealTimePushKO extends React.Component {
	componentDidUpdate() {
		dotnetify.ko.init();
	}
	render() {
		return (
			<div data-vm="RealTimePush">
				<p data-bind="text: Greetings" />
				<p data-bind="text: ServerTime" />
			</div>
		);
	}
}

class ServerUpdate extends React.Component {
	constructor(props) {
		super(props);
		this.vm = dotnetify.react.connect('ServerUpdate', this);
		this.state = { Greetings: '', firstName: '', lastName: '' };
	}
	componentWillUnmount() {
		this.vm.$destroy();
	}
	render() {
		const handleFirstName = e => this.setState({ firstName: e.target.value });
		const handleLastName = e => this.setState({ lastName: e.target.value });
		const handleSubmit = () => this.vm.$dispatch({ Submit: { FirstName: this.state.firstName, LastName: this.state.lastName } });
		return (
			<div>
				<div>{this.state.Greetings}</div>
				<input type="text" value={this.state.firstName} onChange={handleFirstName} />
				<input type="text" value={this.state.lastName} onChange={handleLastName} />
				<button onClick={handleSubmit}>Submit</button>
			</div>
		);
	}
}

window.ServerUpdate = {
	firstName: ko.observable(),
	lastName: ko.observable(),
	submit: function() {
		this.Submit({ FirstName: this.firstName(), LastName: this.lastName() });
	}
};

class ServerUpdateKO extends React.Component {
	componentDidUpdate() {
		dotnetify.ko.init();
	}
	render() {
		return (
			<div data-vm="ServerUpdate">
				<div data-bind="text: Greetings" />
				<input type="text" data-bind="value: firstName" />
				<input type="text" data-bind="value: lastName" />
				<button data-bind="vmCommand: submit">Submit</button>
			</div>
		);
	}
}

export default withTheme(Overview);

class HelloWorld extends React.Component {
	constructor(props) {
		super(props);
		dotnetify.react.connect('HelloWorld', this);
		this.state = { Greetings: '', ServerTime: '' };
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

ReactDOM.render(<HelloWorld />, document.getElementById('App'));

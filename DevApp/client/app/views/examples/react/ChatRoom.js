import React from 'react';
import PropTypes from 'prop-types';
import dotnetify, { Scope } from 'dotnetify';
import TextBox from '../components/TextBox';
import { ChatRoomCss } from '../components/css';

class ChatLobby extends React.Component {
	constructor(props) {
		super(props);
	}

	render() {
		return (
			<ChatRoomCss>
				<ChatRoom />
			</ChatRoomCss>
		);
	}
}

class ChatRoom extends React.Component {
	static contextTypes = { connect: PropTypes.func };

	constructor(props, context) {
		super(props, context);
		this.state = { Users: [], Messages: [], message: '' };

		this.vm = dotnetify.react.connect('ChatRoomVM', this);
	}

	componentDidMount() {
		this.vm.$dispatch({ AddUser: null });
	}

	componentWillUnmount() {
		this.vm.$dispatch({ RemoveUser: null });
		this.vm.$destroy();
	}

	sendMessage(text) {
		this.dispatchState({
			Send: {
				Text: text,
				Date: new Date()
			}
		});
	}

	render() {
		return (
			<React.Fragment>
				<nav>{this.state.Users.map(user => <div key={user.Id}>{user.Name}</div>)}</nav>
				<section>
					<div>
						{this.state.Messages.map(msg => (
							<div>
								<div>{msg.Date}</div>
								<div>{msg.UserName}</div>
								<div>{msg.Browser}</div>
								<div>{msg.Text}</div>
							</div>
						))}
					</div>
					<TextBox
						placeholder="Type message"
						value={this.state.message}
						onChange={value => this.setState({ message: value })}
						onUpdate={value => this.sendMessage(value)}
					/>
				</section>
			</React.Fragment>
		);
	}
}

export default ChatLobby;

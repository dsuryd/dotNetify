import React from 'react';
import PropTypes from 'prop-types';
import dotnetify, { Scope } from 'dotnetify';
import TextBox from '../components/TextBox';
import { ChatRoomCss } from '../components/css';

class ChatRoom extends React.Component {
	static contextTypes = { connect: PropTypes.func };

	constructor(props, context) {
		super(props, context);
		this.state = { Users: [], Messages: [], message: '' };

		this.vm = dotnetify.react.connect('ChatRoomVM', this);
		this.dispatchState = state => this.vm.$dispatch(state);

		this.scrollToBottom = () => this.bottomElem.scrollIntoView({ behavior: 'smooth' });
	}

	componentDidMount() {
		this.dispatchState({ AddUser: null });
		this.scrollToBottom();
	}

	componentDidUpdate() {
		this.scrollToBottom();
	}

	componentWillUnmount() {
		this.dispatchState({ RemoveUser: null });
		this.vm.$destroy();
	}

	getUserName(userId) {
		const user = this.state.Users.find(x => x.Id === userId);
		return user ? user.Name : null;
	}

	sendMessage(text) {
		var match = /name is ([A-z]+)/i.exec(text);
		this.dispatchState({
			Send: {
				Text: text,
				Date: new Date(),
				UserName: match ? match[1] : ''
			}
		});
		this.setState({ message: '' });
	}

	render() {
		return (
			<ChatRoomCss>
				<div className="chatPanel">
					<nav>
						{this.state.Users.map(user => (
							<p key={user.Id}>
								<b>{user.Name}</b>
								<span>{user.IpAddress}</span>
								<span>{user.Browser}</span>
							</p>
						))}
					</nav>
					<section>
						<div>
							{this.state.Messages.map(msg => (
								<div key={msg.Id}>
									<div>
										<span>{this.getUserName(msg.UserId) || msg.UserName}</span>
										<span>{new Date(msg.Date).toLocaleString()}</span>
									</div>
									<div>{msg.Text}</div>
								</div>
							))}
							<div style={{ float: 'left', clear: 'both' }} ref={el => (this.bottomElem = el)} />
						</div>
						<TextBox
							placeholder="Type your message here"
							value={this.state.message}
							onChange={value => this.setState({ message: value })}
							onUpdate={value => this.sendMessage(value)}
						/>
					</section>
				</div>
				<footer>* Hint: type 'my name is ___' to introduce yourself.</footer>
			</ChatRoomCss>
		);
	}
}

export default ChatRoom;

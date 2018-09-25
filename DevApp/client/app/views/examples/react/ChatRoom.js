import React from 'react';
import dotnetify from 'dotnetify';
import TextBox from '../components/TextBox';
import { ChatRoomCss } from '../components/css';

class ChatRoom extends React.Component {
	constructor(props) {
		super(props);
		this.state = { Messages: [], message: '' };

		this.vm = dotnetify.react.connect('ChatLobbyVM', this);
		this.dispatchState = state => this.vm.$dispatch(state);
	}

	componentWillUnmount() {
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
			<ChatRoomCss>
				<section>
					{this.state.Messages.map(msg => (
						<div>
							<div>{msg.Date}</div>
							<div>{msg.UserName}</div>
							<div>{msg.Browser}</div>
							<div>{msg.Text}</div>
						</div>
					))}
				</section>
				<footer>
					<TextBox
						placeholder="Type message"
						value={this.state.message}
						onChange={value => this.setState({ message: value })}
						onUpdate={value => this.sendMessage(value)}
					/>
				</footer>
			</ChatRoomCss>
		);
	}
}

export default ChatRoom;

import React from 'react';
import PropTypes from 'prop-types';
import dotnetify, { Scope } from 'dotnetify';
import TextBox from '../components/TextBox';
import { ChatRoomCss } from '../components/css';

class ChatLobby extends React.Component {
  constructor(props) {
    super(props);
    this.state = { Users: [], Messages: [], message: '' };

    this.vm = dotnetify.react.connect('ChatLobbyVM', this);
  }

  componentDidMount() {
    this.vm.$dispatch({ AddUser: null });
  }

  componentWillUnmount() {
    this.vm.$dispatch({ RemoveUser: null });
    this.vm.$destroy();
  }

  render() {
    return (
      <ChatRoomCss>
        <nav>{this.state.Users.map(user => <div key={user.Id}>{user.Name}</div>)}</nav>
        <Scope vm="ChatLobbyVM">
          <ChatRoom />
        </Scope>
      </ChatRoomCss>
    );
  }
}

class ChatRoom extends React.Component {
  static contextTypes = { connect: PropTypes.func };

  constructor(props, context) {
    super(props, context);
    this.state = { Users: [], Messages: [], message: '' };

    this.context.connect('ChatRoomVM', this);
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
    );
  }
}

export default ChatLobby;

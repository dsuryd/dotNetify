import React from 'react';
import dotnetify from 'dotnetify';
import TextBox from '../components/TextBox';
import { ChatRoomCss } from '../components/css';

class ChatRoom extends React.Component {
  constructor(props) {
    super(props);
    this.state = { Users: [], Messages: [], message: '' };
    this.scrollToBottom = () => this.bottomElem.scrollIntoView({ behavior: 'smooth' });

    this.vm = dotnetify.react.connect('ChatRoomVM', this);
    this.dispatchState = state => this.vm.$dispatch(state);
  }

  componentDidMount() {
    this.correlationId = `${Math.random()}`;
    this.dispatchState({ AddUser: this.correlationId });
    this.scrollToBottom();
  }

  componentDidUpdate() {
    this.scrollToBottom();
    if (this.state.PrivateMessage) {
      let message = this.state.PrivateMessage;
      message.Text = '(private) ' + message.Text;
      message.private = true;
      this.setState({ Messages: this.state.Messages.concat(message) });
      this.setState({ PrivateMessage: null });
    }
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
      SendMessage: {
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
                <b className={user.CorrelationId == this.correlationId ? 'myself' : ''}>{user.Name}</b>
                <span>{user.IpAddress}</span>
                <span>{user.Browser}</span>
              </p>
            ))}
          </nav>
          <section>
            <div>
              {this.state.Messages.map((msg, idx) => (
                <div key={idx}>
                  <div>
                    <span>{this.getUserName(msg.UserId) || msg.UserName}</span>
                    <span>{new Date(msg.Date).toLocaleString()}</span>
                  </div>
                  <div className={msg.private ? 'private' : ''}>{msg.Text}</div>
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
        <footer>
          <div>* Hint:</div>
          <ul>
            <li>type 'my name is ___' to introduce yourself</li>
            <li>type '&lt;username&gt;: ___' to send private message</li>
          </ul>
        </footer>
      </ChatRoomCss>
    );
  }
}

export default ChatRoom;

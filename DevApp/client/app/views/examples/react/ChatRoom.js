import React from 'react';
import dotnetify from 'dotnetify';
import TextBox from '../components/TextBox';
//import { ChatRoomCss } from "../components/css";

class ChatRoom extends React.Component {
  constructor(props) {
    super(props);

    // Connect this component to the back-end view model.
    this.vm = dotnetify.react.connect('ChatLobbyVM', this);

    // Set up function to dispatch state to the back-end.
    this.dispatchState = state => this.vm.$dispatch(state);
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
    return <div />;
  }
}

export default ChatRoom;

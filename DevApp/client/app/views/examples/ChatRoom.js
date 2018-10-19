import React from 'react';
import ChatRoomReact from './react/ChatRoom';
import ChatRoomVue from './vue/adapters/ChatRoom';
import Example from './components/Example';

export default _ => <Example vm="ChatRoomExample" react={<ChatRoomReact />} vue={<ChatRoomVue />} />;

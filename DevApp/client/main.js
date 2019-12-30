import React from 'react';
import ReactDOM from 'react-dom';
import dotnetify from 'dotnetify';
import App from './app/views/App';
import './app/styles/app.css';
import './app/styles/prism.css';
import * as views from './app/views';
import 'dotnetify-elements/dotnetify-elements.css';

//import { MessagePackHubProtocol } from '@aspnet/signalr-protocol-msgpack';
//const protocol = new MessagePackHubProtocol();
//dotnetify.hubOptions.connectionBuilder = builder => builder.withHubProtocol(protocol);

dotnetify.debug = true;

// Import all the routeable views into the global window variable.
Object.assign(window, { ...views });

ReactDOM.render(<App />, document.getElementById('App'));

import React from 'react';
import ReactDOM from 'react-dom';
import dotnetify from 'dotnetify';
import App from './app/views/App';
import './app/styles/app.css';
import './app/styles/prism.css';
import * as views from './app/views';
import 'dotnetify-elements/dotnetify-elements.css';

//import { enableSsr } from 'dotnetify';
//enableSsr();

dotnetify.debug = true;

// Enable this to switch from JSON to MessagePack protocol
/*
import { MessagePackHubProtocol } from '@aspnet/signalr-protocol-msgpack';
const protocol = new MessagePackHubProtocol();
dotnetify.hubOptions.connectionBuilder = builder => builder.withHubProtocol(protocol);
 */

// Import all the routeable views into the global window variable.
Object.assign(window, { ...views });

ReactDOM.hydrate(<App />, document.getElementById('App'));

import { hydrateRoot } from "react-dom/client";
import dotnetify, { createWebSocketHub } from "dotnetify";
import App from "./app/views/App";
import "./app/styles/app.css";
import "./app/styles/prism.css";
import * as views from "./app/views";
import "../dist/react/v18-compatibility";

// ** Uncomment this to enable SSR.
//import { enableSsr } from 'dotnetify';
//enableSsr();

dotnetify.debug = true;

// ** Uncomment this to enable integration with 3rd party websocket server like AWS API gateway **
//dotnetify.hub = createWebSocketHub("wss://ovcgrr6x5g.execute-api.us-east-1.amazonaws.com/sandbox");
dotnetify.hub = createWebSocketHub("ws://localhost:3000");

// ** Uncomment this to switch SignalR protocol from JSON to MessagePack **
/*
import { MessagePackHubProtocol } from '@microsoft/signalr-protocol-msgpack';
const protocol = new MessagePackHubProtocol();
dotnetify.hubOptions.connectionBuilder = builder => builder.withHubProtocol(protocol);
 */

// Import all the routeable views into the global window variable.
Object.assign(window, { ...views });

const container = document.getElementById("App");
hydrateRoot(container, <App />);

// ** FOR DEV TESTING ONLY **
// import TestApp from "./app/views/examples/react/SecurePage";
// createRoot(container).render(<TestApp />);

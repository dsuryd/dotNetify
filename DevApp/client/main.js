import { hydrateRoot, createRoot } from "react-dom/client";
import dotnetify, { createWebSocketHub } from "dotnetify";
import App from "./app/views/App";
import "./app/styles/app.css";
import "./app/styles/prism.css";
import * as views from "./app/views";
import "../dist/react/v18-compatibility";

dotnetify.debug = true;

// ** Enable server-side rendering **
// import { enableSsr } from 'dotnetify';
// enableSsr();

// ** Enable integration with 3rd party websocket server like AWS API gateway **
if (process.env.ENABLE_AWS_INTEGRATION) {
  dotnetify.hub = createWebSocketHub(process.env.AWS_API_GATEWAY);
  //dotnetify.hub = createWebSocketHub(process.env.LOCAL_WS_SERVER);
}

// ** Switch SignalR protocol from JSON to MessagePack **
// import { MessagePackHubProtocol } from '@microsoft/signalr-protocol-msgpack';
// const protocol = new MessagePackHubProtocol();
// dotnetify.hubOptions.connectionBuilder = builder => builder.withHubProtocol(protocol);

// Import all the routeable views into the global window variable.
Object.assign(window, { ...views });

const container = document.getElementById("App");
hydrateRoot(container, <App />);

// ** DEV TESTING **
//import TestApp from "./app/views/examples/react/ChatRoom";
//createRoot(container).render(<TestApp />);

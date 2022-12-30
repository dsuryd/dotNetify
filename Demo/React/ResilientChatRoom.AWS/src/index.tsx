import React from "react";
import { createRoot } from "react-dom/client";
import dotnetify from "dotnetify";
import { ChatRoom } from "./ChatRoom";

dotnetify.debug = true;
dotnetify.hub = dotnetify.createWebSocketHub(process.env.AWS_WEBSOCKET_URL!);

createRoot(document.getElementById("App") as Element).render(<ChatRoom />);

"use strict";

const http = require("http"),
  axios = require("axios"),
  express = require("express"),
  bodyParser = require("body-parser"),
  uuid = require("uuid"),
  WebSocket = require("ws");

const wsServer = http.createServer(express());
const wsServerPort = 3000;

const httpServer = express();
const httpServerPort = 3010;

const dotnetifyMessageUrl = "http://localhost:5000/api/dotnetify/vm";
const dotnetifyDisconnectUrl = "http://localhost:5000/api/dotnetify/vm/disconnect";

let wsClients = [];

const wss = new WebSocket.Server({ server: wsServer });
wss.on("connection", ws => {
  ws.id = uuid.v4();
  console.log(ws.id, "open");
  wsClients.push(ws);

  ws.on("message", message => {
    const data = {
      connectionId: ws.id,
      payload: JSON.parse(message.toString())
    };
    console.log(ws.id, data);
    axios.post(dotnetifyMessageUrl, data).then(res => {
      console.log("dotNetify", "message", res.status, res.data);
      ws.send(JSON.stringify(res.data));
    });
  });

  ws.on("close", () => {
    console.log(ws.id, "close");
    const data = { connectionId: ws.id };
    axios.post(dotnetifyDisconnectUrl, data).then(res => console.log("dotNetify", "closed", res.status));
    wsClients = wsClients.filter(x => x !== ws);
  });

  ws.on("error", err => console.log(ws.id, "error", err));
});

wss.on("error", err => console.error("server error", err));

wsServer.listen(wsServerPort, () => {
  console.log(`Websocket server started on port ` + wsServerPort);
});

// Connection callback.
httpServer.use(bodyParser.json());
httpServer.use(bodyParser.urlencoded({ extended: true }));
httpServer.post("/:id", (req, res) => {
  console.log(req.params.id, "callback", req.body);
  const ws = wsClients.find(x => x.id === req.params.id);
  if (ws) {
    ws.send(JSON.stringify(req.body));
    res.sendStatus(200);
  } else {
    res.sendStatus(404);
  }
});

httpServer.listen(httpServerPort, () => {
  console.log(`Http server started on port ` + httpServerPort);
});

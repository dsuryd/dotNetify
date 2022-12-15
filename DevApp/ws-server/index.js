"use strict";

const http = require("http"),
  axios = require("axios"),
  express = require("express"),
  uuid = require("uuid"),
  WebSocket = require("ws");

const serverPort = 3000;
const server = http.createServer(express());

const dotnetifyUrl = "http://localhost:5000/api/dotnetify/vm";

const wss = new WebSocket.Server({ server });
wss.on("connection", ws => {
  ws.id = uuid.v4();
  console.log(ws.id, "open");

  const data = { connectionId: ws.id, state: "open" };
  axios.post(dotnetifyUrl, data).then(res => console.log("dotNetify", "open", res.status));

  ws.on("message", message => {
    const data = {
      connectionId: ws.id,
      payload: JSON.parse(message.toString())
    };
    console.log(ws.id, data);
    axios.post(dotnetifyUrl, data).then(res => {
      console.log("dotNetify", "message", res.status, res.data);
      ws.send(JSON.stringify(res.data));
    });
  });

  ws.on("close", () => {
    console.log(ws.id, "close");
  });

  ws.on("error", err => console.log(ws.id, "error", err));
});

wss.on("error", err => console.error("server error", err));

server.listen(serverPort, () => {
  console.log(`Websocket server started on port ` + serverPort);
});

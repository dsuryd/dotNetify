import React, { useEffect } from "react";
import { useConnect } from "dotnetify";

type Payload = {
  ConnectionId: string;
  Time: Date;
  Sequence: number;
  AvgLatency: number;
};

interface State {
  Ping: Payload;
  PingInterval: number;
}

let count = 0;

export const Echo = () => {
  const { vm, state } = useConnect<State>("EchoVM", {}, { vmArg: { PingInterval: 200 }, headers: { Authentication: "Bearer xyz" } });

  useEffect(() => {
    state.Ping && vm?.$dispatch({ Pong: state.Ping });
  }, [state]);

  const { Ping } = state;
  if (!Ping) return <></>;

  return (
    <div>
      <p>Connection ID: {Ping.ConnectionId}</p>
      <p>Time: {Ping.Time}</p>
      <p>Sequence: {Ping.Sequence}</p>
      <p>Avg Latency: {Ping.AvgLatency}</p>
    </div>
  );
};

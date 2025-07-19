import React, { useEffect, useRef } from "react";
import { useConnect } from "dotnetify";

type Payload = {
  MessageId: string;
  SenderId: string;
  Time: Date;
  Sequence: number;
};

interface State {
  Messages: Payload[];
  ChatGroupName: string;
}

let count = 0;
const senderId = Math.random().toString(36).substring(2);

export const ChatRoom = () => {
  let listRef = useRef<any>();

  const { vm, state } = useConnect<State>("ChatRoomVM", {}, { vmArg: { ChatRoom: "group1" }, headers: { Authentication: "Bearer xyz" } });

  useEffect(() => {
    setInterval(() => {
      vm?.$dispatch({
        SendMessage: {
          MessageId: `${senderId}.${++count}`,
          SenderId: senderId,
          Time: new Date().toUTCString(),
          Sequence: count
        }
      });
    }, 1000);
  }, [vm]);

  useEffect(() => {
    listRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [state]);

  const { Messages, ChatGroupName } = state;
  if (!Messages) return <></>;

  return (
    <div>
      <p>Group Name: {ChatGroupName}</p>
      <div style={{ height: "90%", overflowY: "scroll" }}>
        <table>
          <tbody>
            {Messages.map(x => (
              <tr key={x.MessageId}>
                <td>Message ID: {x.MessageId}</td>
                <td>Sender ID: {x.SenderId}</td>
                <td>Time: {x.Time}</td>
                <td>Sequence: {x.Sequence}</td>
              </tr>
            ))}
          </tbody>
        </table>
        <div ref={listRef} />
      </div>
    </div>
  );
};

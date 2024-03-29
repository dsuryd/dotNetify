import React, { useRef, useState, useEffect } from "react";
import { useConnect } from "dotnetify";
import TextBox from "./TextBox";
import { ChatRoomView } from "./ChatRoom.style";

type User = {
  CorrelationId: string;
  Id: string;
  Name: string;
  IpAddress: string;
  Browser: string;
};

type Message = {
  UserId: string;
  UserName: string;
  Date: string;
  Text: string;

  // Local state.
  private: boolean;
};

type ChatRoomState = {
  Users: User[];
  Messages: Message[];
  PrivateMessage: Message;

  // Local state.
  message: string;
};

const initialState = { Users: [], Messages: [], message: "" };

export const ChatRoom = () => {
  const { vm, state, setState } = useConnect<ChatRoomState>("ChatRoomVM", initialState);
  const bottomRef = useRef<any>();
  const [correlationId] = useState(`${Math.random()}`);

  useEffect(() => () => vm && vm.$dispatch({ RemoveUser: null }), []);

  useEffect(() => {
    if (vm && !state.Users.some(x => x.CorrelationId == correlationId)) {
      vm.$dispatch({ AddUser: correlationId });
    }

    bottomRef.current.scrollIntoView({ behavior: "smooth" });

    if (state.PrivateMessage) {
      let message = state.PrivateMessage;
      message.Text = "(private) " + message.Text;
      message.private = true;
      setState({ Messages: state.Messages.concat(message) });
      setState({ PrivateMessage: null });
    }
  }, [state]);

  const getUserName = (userId: string) => {
    const user = state.Users.find(x => x.Id === userId);
    return user ? user.Name : null;
  };

  const sendMessage = (text: string) => {
    const match = /name is ([A-z]+)/i.exec(text);
    vm.$dispatch({
      SendMessage: {
        Text: text,
        Date: new Date(),
        UserName: match ? match[1] : ""
      }
    });
    setState({ message: "" });
  };

  return (
    <ChatRoomView>
      <div className="chatPanel">
        <nav>
          {state.Users.map(user => (
            <p key={user.Id}>
              <b className={user.CorrelationId == correlationId ? "myself" : ""}>{user.Name}</b>
              <span>{user.IpAddress}</span>
              <span>{user.Browser}</span>
            </p>
          ))}
        </nav>
        <section>
          <div>
            {state.Messages.map((msg, idx) => (
              <div key={idx}>
                <div>
                  <span>{getUserName(msg.UserId) || msg.UserName}</span>
                  <span>{new Date(msg.Date).toLocaleString()}</span>
                </div>
                <div className={msg.private ? "private" : ""}>{msg.Text}</div>
              </div>
            ))}
            <div style={{ float: "left", clear: "both" }} ref={bottomRef} />
          </div>
          <TextBox
            placeholder="Type your message here"
            value={state.message}
            onChange={value => setState({ message: value })}
            onUpdate={value => sendMessage(value)}
          />
        </section>
      </div>
      <footer>
        <div>* Hint:</div>
        <ul>
          <li>type 'my name is ___' to introduce yourself</li>
          <li>type '&lt;username&gt;: ___' to send private message</li>
        </ul>
      </footer>
    </ChatRoomView>
  );
};

export default ChatRoom;

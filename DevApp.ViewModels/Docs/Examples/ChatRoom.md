##### ChatRoom.js

```jsx
import React from "react";
import dotnetify from "dotnetify";
import TextBox from "../components/TextBox";
import { ChatRoomCss } from "../components/css";

export const ChatRoom = () => {
  const { vm, state, setState } = useConnect("ChatRoomVM", { Users: [], Messages: [], message: "" });
  const bottomRef = useRef();
  const [correlationId] = useState(`${Math.random()}`);

  useEffect(() => {
    return () => vm.$dispatch({ RemoveUser: null });
  }, []);

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

  const getUserName = userId => {
    const user = state.Users.find(x => x.Id === userId);
    return user ? user.Name : null;
  };

  const sendMessage = text => {
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
    <ChatRoomCss>
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
    </ChatRoomCss>
  );
};
```

##### ChatRoomVM.cs

```csharp
public class ChatMessage
{
  public int Id { get; set; }
  public string UserId { get; set; }
  public string UserName { get; set; }
  public DateTimeOffset Date { get; set; }
  public string Text { get; set; }
}

public class ChatUser
{
  private static int _counter = 0;

  public string Id { get; set; }
  public string CorrelationId { get; set; }
  public string Name { get; set; }
  public string IpAddress { get; set; }
  public string Browser { get; set; }

  public ChatUser(IConnectionContext connectionContext, string correlationId)
  {
      Id = connectionContext.ConnectionId;
      CorrelationId = correlationId;
      Name = $"user{Interlocked.Increment(ref _counter)}";
      IpAddress = connectionContext.HttpConnection.RemoteIpAddress.ToString();

      var browserInfo = Parser.GetDefault().Parse(connectionContext.HttpRequestHeaders.UserAgent);
      if (browserInfo != null)
        Browser = $"{browserInfo.UserAgent.Family}/{browserInfo.OS.Family} {browserInfo.OS.Major}";
  }
}

public class ChatRoomVM : MulticastVM
{
  private readonly IConnectionContext _connectionContext;

  [ItemKey(nameof(ChatMessage.Id))]
  public List<ChatMessage> Messages { get; } = new List<ChatMessage>();

  [ItemKey(nameof(ChatUser.Id))]
  public List<ChatUser> Users { get; } = new List<ChatUser>();

  public Action<ChatMessage> SendMessage => chat =>
  {
      string userId = _connectionContext.ConnectionId;
      chat.Id = Messages.Count + 1;
      chat.UserId = userId;
      chat.UserName = UpdateUserName(userId, chat.UserName);

      var privateMessageUser = Users.FirstOrDefault(x => chat.Text.StartsWith($"{x.Name}:"));
      if (privateMessageUser != null)
        base.Send(new List<string> { privateMessageUser.Id, userId }, "PrivateMessage", chat);
      else
      {
        lock (Messages)
        {
            Messages.Add(chat);
            this.AddList(nameof(Messages), chat);
        }
      }
  };

  public Action<string> AddUser => correlationId =>
  {
      var user = new ChatUser(_connectionContext, correlationId);
      lock (Users)
      {
        Users.Add(user);
        this.AddList(nameof(Users), user);
      }
  };

  public Action RemoveUser => () =>
  {
      lock (Users)
      {
        var user = Users.FirstOrDefault(x => x.Id == _connectionContext.ConnectionId);
        if (user != null)
        {
            Users.Remove(user);
            this.RemoveList(nameof(Users), user.Id);
        }
      }
  };

  public ChatRoomVM(IConnectionContext connectionContext)
  {
      _connectionContext = connectionContext;
  }

  public override void Dispose()
  {
      RemoveUser();
      PushUpdates();
      base.Dispose();
  }

  private string UpdateUserName(string userId, string userName)
  {
      lock (Users)
      {
        var user = Users.FirstOrDefault(x => x.Id == userId);
        if (user != null)
        {
            if (!string.IsNullOrEmpty(userName))
            {
              user.Name = userName;
              this.UpdateList(nameof(Users), user);
            }
            return user.Name;
        }
      }
      return userId;
  }
}
```

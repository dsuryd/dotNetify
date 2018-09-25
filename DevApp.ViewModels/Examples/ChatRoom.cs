using DotNetify.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using UAParser;

namespace DotNetify.DevApp
{
   public class ChatRoomExample : BaseVM
   {
      public ChatRoomExample()
      {
         var markdown = new Markdown("DotNetify.DevApp.Docs.Examples.ChatRoom.md");

         AddProperty("ViewSource", markdown.GetSection(null, "ChatRoomVM.cs"))
            .SubscribeTo(AddInternalProperty<string>("Framework").Select(GetViewSource));

         AddProperty("ViewModelSource", markdown.GetSection("ChatRoomVM.cs"));
      }

      private string GetViewSource(string framework)
      {
         return new Markdown("DotNetify.DevApp.Docs.Examples.ChatRoom.md");
      }
   }

   public class ChatMessage
   {
      public int Id { get; set; }
      public string UserId { get; set; }
      public DateTimeOffset Date { get; set; }
      public string Text { get; set; }
   }

   public class ChatUser
   {
      private static int _nameGenerator = 1;

      public string Id { get; set; }
      public string Name { get; set; }
      public string IpAddress { get; set; }
      public string Browser { get; set; }
      public string Room { get; set; } = "Lobby";

      public ChatUser(IConnectionContext connectionContext)
      {
         Id = connectionContext.HttpConnection.ConnectionId;
         Name = $"user{Interlocked.Increment(ref _nameGenerator)}";
         IpAddress = connectionContext.HttpConnection.RemoteIpAddress.ToString();

         var browserInfo = Parser.GetDefault().Parse(connectionContext.HttpRequestHeaders.UserAgent);
         if (browserInfo != null)
            Browser = $"{browserInfo.UserAgent.Family}/{browserInfo.OS.Family} {browserInfo.OS.Major}";
      }
   }

   public class ChatLobbyVM : MulticastVM
   {
      private readonly IConnectionContext _connectionContext;

      private Dictionary<string, ChatRoomVM> _chatRooms = new Dictionary<string, ChatRoomVM>();

      public override bool IsMember => true;

      public List<ChatUser> Users { get; } = new List<ChatUser>();

      public string Users_itemKey => nameof(ChatUser.Id);

      public Action<object> AddUser => _ =>
      {
         var user = new ChatUser(_connectionContext);
         Users.Add(user);
         this.AddList(nameof(Users), user);
         if (_chatRooms.ContainsKey(user.Room))
            _chatRooms[user.Room].AddUser(user);
      };

      public Action RemoveUser => () =>
      {
         var user = Users.FirstOrDefault(x => x.Id == _connectionContext.HttpConnection.ConnectionId);
         Users.Remove(user);
         this.RemoveList(nameof(Users), user);
         if (_chatRooms.ContainsKey(user.Room))
            _chatRooms[user.Room].RemoveUser(user);
      };

      public Action<string> ChangeRoom => room =>
      {
         var user = Users.FirstOrDefault(x => x.Id == _connectionContext.HttpConnection.ConnectionId);
         user.Room = room;
         this.UpdateList(nameof(Users), user);
      };

      public ChatLobbyVM(IConnectionContext connectionContext)
      {
         _connectionContext = connectionContext;
      }

      public override BaseVM GetSubVM(string vmTypeName, string vmInstanceId)
      {
         if (vmTypeName != nameof(ChatRoomVM)) return null;

         var room = vmInstanceId ?? "Lobby";
         lock (_chatRooms)
         {
            if (!_chatRooms.ContainsKey(room))
               _chatRooms.Add(vmInstanceId, new ChatRoomVM(_connectionContext, Users.Where(x => x.Room == room).ToList()));

            return _chatRooms[room];
         }
      }
   }

   public class ChatRoomVM : BaseVM
   {
      private readonly IConnectionContext _connectionContext;

      public List<ChatMessage> Messages { get; } = new List<ChatMessage>();

      public string Messages_itemKey => nameof(ChatMessage.Id);

      public List<ChatUser> Users { get; private set; }

      public string Users_itemKey => nameof(ChatUser.Id);

      public Action<ChatMessage> Send => chat =>
      {
         lock (Messages)
         {
            chat.Id = Messages.Count + 1;
            chat.UserId = _connectionContext.HttpConnection.ConnectionId;
            Messages.Add(chat);
            this.AddList(nameof(Messages), chat);
         }
      };

      public Action<ChatUser> AddUser => user =>
      {
         if (Users.Any(x => x.Id == user.Id)) return;
         Users.Add(user);
         this.AddList(nameof(Users), user);
      };

      public Action<ChatUser> RemoveUser => user =>
      {
         var roomUser = Users.FirstOrDefault(x => x.Id == user.Id);
         if (roomUser != null)
         {
            Users.Remove(roomUser);
            this.RemoveList(nameof(Users), roomUser);
         }
      };

      public ChatRoomVM(IConnectionContext connectionContext, List<ChatUser> users)
      {
         _connectionContext = connectionContext;
         Users = users;
      }
   }
}
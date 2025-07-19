using DotNetify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using UAParser;

namespace DevApp.ViewModels
{
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

      public List<ChatMessage> Messages { get; } = new List<ChatMessage>();

      public string Messages_itemKey => nameof(ChatMessage.Id);

      public List<ChatUser> Users { get; } = new List<ChatUser>();

      public string Users_itemKey => nameof(ChatUser.Id);

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
}
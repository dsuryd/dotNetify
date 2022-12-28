using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetify.Elements;
using MemoryPack;
using Microsoft.Extensions.Caching.Distributed;
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
         return framework == "Vue" ?
            new Markdown("DotNetify.DevApp.Docs.Vue.Examples.ChatRoom.md") :
            new Markdown("DotNetify.DevApp.Docs.Examples.ChatRoom.md").GetSection(null, "ChatRoomVM.cs");
      }
   }

   [MemoryPackable]
   public partial class ChatMessage
   {
      public long Id { get; set; }
      public string UserId { get; set; }
      public string UserName { get; set; }
      public DateTimeOffset Date { get; set; }
      public string Text { get; set; }
   }

   [MemoryPackable]
   public partial class ChatUser
   {
      internal static int _counter = 0;

      public string Id { get; set; }
      public string CorrelationId { get; set; }

      public string Name { get; set; }
      public string IpAddress { get; set; }
      public string Browser { get; set; }

      [MemoryPackConstructor]
      public ChatUser()
      { }

      public ChatUser(IConnectionContext connectionContext, string correlationId)
      {
         Id = connectionContext.ConnectionId;
         CorrelationId = correlationId;
         Name = $"user{Interlocked.Increment(ref _counter)}";
         IpAddress = connectionContext.HttpConnection.RemoteIpAddress?.ToString();

         var userAgent = connectionContext.HttpRequestHeaders?.UserAgent;
         if (userAgent != null)
         {
            var browserInfo = Parser.GetDefault().Parse(userAgent);
            if (browserInfo != null)
               Browser = $"{browserInfo.UA.Family}/{browserInfo.OS.Family} {browserInfo.OS.Major}";
         }
      }
   }

   public class ChatRoomVM : MulticastVM
   {
      private readonly IConnectionContext _connectionContext;
      private readonly IDistributedCache _cache;

      [ItemKey(nameof(ChatMessage.Id))]
      public List<ChatMessage> Messages { get; private set; } = new List<ChatMessage>();

      [ItemKey(nameof(ChatUser.Id))]
      public List<ChatUser> Users { get; private set; } = new List<ChatUser>();

      public ChatRoomVM(IConnectionContext connectionContext, IDistributedCache cache)
      {
         _connectionContext = connectionContext;
         _cache = cache;
      }

      public override void Dispose()
      {
         RemoveUser();
         PushUpdates();
         base.Dispose();
      }

      public override Task OnCreatedAsync()
      {
         // Restore view model state from a distributed cache.
         return LoadStateAsync();
      }

      public void SendMessage(ChatMessage chat)
      {
         string userId = _connectionContext.ConnectionId;
         chat.Id = DateTime.UtcNow.Ticks;
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
               SaveState();
            }
         }
      }

      public void AddUser(string correlationId)
      {
         var user = new ChatUser(_connectionContext, correlationId);
         lock (Users)
         {
            Users.Add(user);
            this.AddList(nameof(Users), user);
            SaveState();
         }
      }

      public void RemoveUser()
      {
         lock (Users)
         {
            var user = Users.FirstOrDefault(x => x.Id == _connectionContext.ConnectionId);
            if (user != null)
            {
               Users.Remove(user);
               this.RemoveList(nameof(Users), user.Id);
               SaveState();
            }
         }
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
                  SaveState();
               }
               return user.Name;
            }
         }
         return userId;
      }

      private void SaveState()
      {
         if (Users.Count > 0)
         {
            var entryOptions = new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) };
            _cache.SetAsync(nameof(ChatRoomVM) + "_users", MemoryPackSerializer.Serialize(Users), entryOptions);
            _cache.SetAsync(nameof(ChatRoomVM) + "_messages", MemoryPackSerializer.Serialize(Messages), entryOptions);
         }
         else
         {
            _cache.Remove(nameof(ChatRoomVM) + "_users");
            _cache.Remove(nameof(ChatRoomVM) + "_messages");
         }
      }

      private async Task LoadStateAsync()
      {
         var bytes = await _cache.GetAsync(nameof(ChatRoomVM) + "_users");
         if (bytes != null)
         {
            Users = MemoryPackSerializer.Deserialize<List<ChatUser>>(bytes);
            ChatUser._counter = Users.Count;
         }

         bytes = await _cache.GetAsync(nameof(ChatRoomVM) + "_messages");
         if (bytes != null)
         {
            Messages = MemoryPackSerializer.Deserialize<List<ChatMessage>>(bytes);
         }
      }
   }
}
using System.Reactive.Linq;
using DotNetify;
using DotNetify.WebApi;
using MemoryPack;
using Microsoft.Extensions.Caching.Distributed;
using UAParser;

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
   public string ConnectionId { get; set; }
   public string CorrelationId { get; set; }

   public string Name { get; set; }
   public string IpAddress { get; set; }
   public string Browser { get; set; }

   [MemoryPackConstructor]
   public ChatUser()
   { }

   public ChatUser(IConnectionContext connectionContext, string correlationId)
   {
      Id = $"userId_{DateTime.UtcNow.Ticks}";
      ConnectionId = connectionContext.ConnectionId;
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

   public void Copy(ChatUser user)
   {
      ConnectionId = user.ConnectionId;
      CorrelationId = user.CorrelationId;
      IpAddress = user.IpAddress;
      Browser = user.Browser;
   }
}

public class ChatRoomVM : MulticastVM
{
   private readonly IConnectionContext _connectionContext;
   private readonly IDistributedCache _cache;
   private readonly IWebApiConnectionCache _connectionCache;

   private static readonly TimeSpan CACHE_EXPIRATION = TimeSpan.FromMinutes(10);

   [ItemKey(nameof(ChatMessage.Id))]
   public List<ChatMessage> Messages { get; private set; } = new List<ChatMessage>();

   [ItemKey(nameof(ChatUser.Id))]
   public List<ChatUser> Users { get; private set; } = new List<ChatUser>();

   public ChatRoomVM(IConnectionContext connectionContext, IDistributedCache cache, IWebApiConnectionCache connectionCache)
   {
      _connectionContext = connectionContext;
      _cache = cache;
      _connectionCache = connectionCache;
   }

   public override void Dispose()
   {
      RemoveUser(_connectionContext.ConnectionId);
      PushUpdates();
      base.Dispose();
   }

   public override async Task OnCreatedAsync()
   {
      // Restore view model state from a distributed cache.
      await LoadStateAsync();

      // If this view model is being restored from previous run, check if the cached users are still connected.
      // If not, update the users list and broadcast it.
      if (Users.Count > 0)
      {
         var connections = await _connectionCache.GetConnectionsAsync();
         if (Users.Any(user => !connections.Any(conn => conn.Id == user.ConnectionId)))
         {
            Users = Users.Where(user => connections.Any(conn => conn.Id == user.ConnectionId)).ToList();

            // Give it a couple seconds delay make sure everything has been initialized.
            _ = Task.Run(async () =>
            {
               await Task.Delay(2000);
               Changed(nameof(Users));
               PushUpdates();
            });
         }
      }
   }

   public void SendMessage(ChatMessage chat)
   {
      var user = Users.FirstOrDefault(user => user.ConnectionId == _connectionContext.ConnectionId);

      chat.Id = DateTime.UtcNow.Ticks;
      chat.UserId = user.Id;
      chat.UserName = UpdateUserName(user.Id, chat.UserName);

      var privateMessageUser = Users.FirstOrDefault(x => chat.Text.StartsWith($"{x.Name}:"));
      if (privateMessageUser != null)
         base.Send(new List<string> { privateMessageUser.ConnectionId, user.ConnectionId }, "PrivateMessage", chat);
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
      if (Users.Any(u => u.CorrelationId == correlationId))
         return;

      var user = new ChatUser(_connectionContext, correlationId);
      lock (Users)
      {
         Users.Add(user);
         this.AddList(nameof(Users), user);
         SaveState();
      }
   }

   public void RemoveUser(string connectionId)
   {
      lock (Users)
      {
         var user = Users.FirstOrDefault(x => x.ConnectionId == connectionId);
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
         var entryOptions = new DistributedCacheEntryOptions { SlidingExpiration = CACHE_EXPIRATION };
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
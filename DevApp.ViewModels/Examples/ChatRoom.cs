using DotNetify.Elements;
using System.Linq;
using System.Reactive.Linq;

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

   public class ChatLobbyVM : MulticastVM
   {
      private readonly IConnectionContext _connectionContext;

      public override bool IsMember => true;

      public ChatLobbyVM(IConnectionContext connectionContext)
      {
         _connectionContext = connectionContext;
         var test = _connectionContext.HttpConnection.LocalIpAddress;
         var test2 = _connectionContext.HttpRequestHeaders["User-Agent"];
      }
   }
}
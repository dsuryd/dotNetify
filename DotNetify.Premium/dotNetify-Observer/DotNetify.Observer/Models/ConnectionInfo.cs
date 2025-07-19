using System;
using System.Security.Principal;

namespace DotNetify.Observer
{
   public class ConnectionInfo
   {
      public ConnectionContext Context { get; set; }
      public ConnectionContext OriginContext { get; set; }
      public DateTimeOffset TimeStamp { get; set; }
      public string CallType { get; set; }
      public string VMId { get; set; }
      public object Data { get; set; }
      public IPrincipal Principal { get; set; }
      public VMController.GroupSend GroupSend { get; set; }

      public bool InMulticast(string connectionId) => GroupSend?.ConnectionIds.Contains(connectionId) == true;
   }
}
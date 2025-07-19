using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;

namespace DotNetify.Testing
{
   internal class HubCallerContextStub : HubCallerContext
   {
      private const string CONNECTION_CONTEXT_TOKEN = "$fwdConnContext";
      private readonly IConnectionContext _clientContext;

      private class ConnectionContext : IConnectionContext
      {
         private readonly Func<IConnectionContext> _contextFunc;

         public string ConnectionId => _contextFunc().ConnectionId;

         public HttpConnection HttpConnection => _contextFunc().HttpConnection;

         public HttpRequestHeaders HttpRequestHeaders => _contextFunc().HttpRequestHeaders;

         public ConnectionContext(Func<IConnectionContext> contextFunc)
         {
            _contextFunc = contextFunc;
         }
      }

      public override string ConnectionId { get; }

      public override string UserIdentifier { get; }

      public override ClaimsPrincipal User { get; }

      public override IDictionary<object, object> Items { get; } = new Dictionary<object, object>();

      public override IFeatureCollection Features { get; } = new FeatureCollection();

      public override CancellationToken ConnectionAborted { get; } = new CancellationToken();

      public override void Abort()
      {
         throw new NotImplementedException();
      }

      public HubCallerContextStub(IConnectionContext clientContext, ClaimsPrincipal user)
      {
         _clientContext = clientContext;
         ConnectionId = clientContext.ConnectionId;
         User = user;

         var httpConnectionFeature = Stubber.Create<IHttpConnectionFeature>()
            .Setup(x => x.ConnectionId).Returns(_clientContext.ConnectionId)
            .Setup(x => x.LocalIpAddress).Returns(_clientContext.HttpConnection.LocalIpAddress)
            .Setup(x => x.RemoteIpAddress).Returns(_clientContext.HttpConnection.RemoteIpAddress)
            .Setup(x => x.LocalPort).Returns(_clientContext.HttpConnection.LocalPort)
            .Setup(x => x.RemotePort).Returns(_clientContext.HttpConnection.RemotePort)
            .Object;

         Features.Set(httpConnectionFeature);
      }

      public IConnectionContext GetConnectionContext()
      {
         return new ConnectionContext(() =>
         {
            if (Items.ContainsKey(CONNECTION_CONTEXT_TOKEN))
               return JsonSerializer.Deserialize<ClientContext.ClientConnectionContext>(Items[CONNECTION_CONTEXT_TOKEN].ToString());

            return _clientContext;
         });
      }
   }
}
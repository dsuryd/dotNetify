using Microsoft.AspNetCore.SignalR;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetify.Testing
{
   /// <summary>
   /// Emulates SignalR hub client context.
   /// </summary>
   internal class ClientContext
   {
      public class ClientConnectionContext : IConnectionContext
      {
         public string ConnectionId { get; set; }
         public HttpConnection HttpConnection { get; set; }
         public HttpRequestHeaders HttpRequestHeaders { get; set; }
      }

      public class ClientProxyStub : IClientProxy
      {
         public ReactiveProperty<object[]> Response { get; set; }

         public Task SendCoreAsync(string method, object[] args, CancellationToken cancellationToken = default)
         {
            Response.OnNext(args);
            return Task.CompletedTask;
         }
      }

      public string ConnectionId { get; }
      public ClientProxyStub ClientProxy { get; }
      public IConnectionContext ConnectionContext { get; }

      public ReactiveProperty<object[]> Response { get; } = new ReactiveProperty<object[]>();

      /// <summary>
      /// Constructor to generate mock client context.
      /// </summary>
      /// <param name="connectionId">SignalR connection ID; auto-generated if null.</param>
      /// <param name="httpConnection">HTTP connection context; auto-generated if null.</param>
      /// <param name="httpRequestHeaders">HTTP request headers; auto-generated if null.</param>
      public ClientContext(string connectionId = null, HttpConnection httpConnection = null, HttpRequestHeaders httpRequestHeaders = null)
      {
         ConnectionId = connectionId ?? Guid.NewGuid().ToString();
         ClientProxy = new ClientProxyStub { Response = Response };

         ConnectionContext = new ClientConnectionContext
         {
            ConnectionId = ConnectionId,
            HttpConnection = httpConnection ?? new HttpConnection(Guid.NewGuid().ToString(), IPAddress.Parse("127.0.0.1"), 0, IPAddress.Parse(GetRandomIpAddress()), 80),
            HttpRequestHeaders = httpRequestHeaders ?? new HttpRequestHeaders(new { }, UserAgent.ChromeWindows10)
         };
      }

      /// <summary>
      /// Generates random IP address for mocking HttpConnection object.
      /// </summary>
      /// <returns></returns>
      private string GetRandomIpAddress()
      {
         var random = new Random();
         return $"{random.Next(1, 255)}.{random.Next(0, 255)}.{random.Next(0, 255)}.{random.Next(0, 255)}";
      }
   }
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DotNetify.Forwarding;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace DotNetify.Observer
{
   public interface IConnectionTracker
   {
      IObservable<ConnectionInfo> Connection { get; }
      IObservable<ConnectionGraph> Graph { get; }
      IObservable<TelemetryData> Telemetry { get; }

      ConnectionGraph GetConnectionGraph();

      void ReceiveHubMessage(IConnectionContext originContext, DotNetifyHubContext hubContext);

      void ReceiveDisconnection(IConnectionContext originContext);

      void ReceiveTelemetry(string hubId, string name, Dictionary<string, object> info);

      void Reset();
   }

   public class ConnectionTracker : IConnectionTracker
   {
      private readonly ILogger _logger;
      private readonly MemoryCache _disconnectionCache = new MemoryCache(new MemoryCacheOptions());
      private readonly ConcurrentDictionary<string, ConnectionInfo> _connectionsById = new ConcurrentDictionary<string, ConnectionInfo>();
      private readonly ConcurrentDictionary<string, ConnectionVertex> _verticesById = new ConcurrentDictionary<string, ConnectionVertex>();
      private readonly ConcurrentDictionary<string, ConnectionEdge> _edgesById = new ConcurrentDictionary<string, ConnectionEdge>();
      private readonly ConcurrentDictionary<string, ConnectionEdge> _groupEdges = new ConcurrentDictionary<string, ConnectionEdge>();
      private readonly ISubject<ConnectionInfo> _connectionSubject = Subject.Synchronize(new Subject<ConnectionInfo>());
      private readonly ISubject<ConnectionGraph> _graphSubject = Subject.Synchronize(new Subject<ConnectionGraph>());
      private readonly ISubject<TelemetryData> _telemetrySubject = Subject.Synchronize(new Subject<TelemetryData>());

      public IObservable<ConnectionInfo> Connection => _connectionSubject;
      public IObservable<ConnectionGraph> Graph => _graphSubject;

      public IObservable<TelemetryData> Telemetry => _telemetrySubject;

      public ConnectionTracker(ILogger<ConnectionTracker> logger)
      {
         _logger = logger;

         License.Check();
      }

      public ConnectionGraph GetConnectionGraph()
      {
         return new ConnectionGraph
         {
            Vertices = _verticesById.Select(x => x.Value).ToList(),
            Edges = _edgesById.Select(x => x.Value).Union(_groupEdges.Values).ToList()
         };
      }

      /// <summary>
      /// Receives a hub message.
      /// </summary>
      /// <param name="context">Connection context of the hub when it received the message.</param>
      /// <param name="hubContext">Current caller context containing the message.</param>
      public void ReceiveHubMessage(IConnectionContext context, DotNetifyHubContext hubContext)
      {
         var info = AddOrUpdateConnectionInfo(context as ConnectionContext, hubContext);
         if (info == null)
            return;

         // Don't process messages from connections that we have been notified of disconnection.
         // This can happen when the messages are received out of sequence due to resource stress.
         lock (_disconnectionCache)
         {
            if (_disconnectionCache.TryGetValue(info.Context.ConnectionId, out object _))
               return;
            if (info.OriginContext != null && _disconnectionCache.TryGetValue(info.OriginContext.ConnectionId, out object _))
               return;
         }

         var graph = new ConnectionGraph();

         string targetId = info.Context.HubId;
         var targetVertex = GetOrAddVertex(ConnectionVertexType.Hub, targetId, info.Context.GetHostName(), info.Context, out bool isNewTarget);

         if (isNewTarget)
            graph.Vertices.Add(targetVertex);

         ConnectionVertex sourceVertex;
         string sourceId;
         bool isNewSource;
         bool isNewEdge = false;

         // Connections with origin context are sent by a forwarding hub.
         if (info.OriginContext != null)
         {
            sourceId = info.OriginContext.HubId;
            sourceVertex = GetOrAddVertex(ConnectionVertexType.Hub, sourceId, info.OriginContext.GetHostName(), info.OriginContext, out isNewSource);
         }
         else
         {
            sourceId = info.Context.ConnectionId;
            sourceVertex = GetOrAddVertex(ConnectionVertexType.Client, sourceId, info.Context.HttpConnection.RemoteIpAddressString, info.Context, out isNewSource);
         }

         if (isNewSource)
            graph.Vertices.Add(sourceVertex);

         if (sourceVertex != null && targetVertex != null)
         {
            Log(info, hubContext, sourceVertex, targetVertex);

            ConnectionEdge edge = GetOrAddEdge(info.Context.ConnectionId, sourceVertex.Id, targetVertex.Id, info, out isNewEdge);
            if (isNewEdge)
               graph.Edges.Add(edge);
            else
               UpdateEdgeInfo(edge, info);

            UpdateThroughput(info, hubContext, sourceVertex, targetVertex);
         }

         if (isNewSource || isNewTarget || isNewEdge)
            _graphSubject.OnNext(graph);
      }

      /// <summary>
      /// Receives a disconnection event from a hub.
      /// </summary>
      /// <param name="context"></param>
      public void ReceiveDisconnection(IConnectionContext context)
      {
         lock (_disconnectionCache)
         {
            var connectionId = context.ConnectionId;
            var originContext = (context as ConnectionContext).GetOriginContext();
            if (originContext != null)
            {
               connectionId = originContext.ConnectionId;
            };

            if (_edgesById.TryRemove(connectionId, out ConnectionEdge edge))
            {
               var graph = new ConnectionGraph { Removed = true };
               graph.Edges.Add(edge);

               bool sourceVertexHasEdge = _edgesById.Any(x => x.Value.SourceId == edge.SourceId);
               bool vertexIsHub = _verticesById.TryGetValue(edge.SourceId, out ConnectionVertex value) && value.Type == ConnectionVertexType.Hub;

               if (!sourceVertexHasEdge && !vertexIsHub && _verticesById.TryRemove(edge.SourceId, out ConnectionVertex vertex))
               {
                  graph.Vertices.Add(vertex);

                  var groupEdges = _groupEdges.Where(x => x.Value.SourceId == vertex.Id || x.Value.TargetId == vertex.Id);
                  graph.Edges.ToList().AddRange(groupEdges.Select(x => x.Value));

                  foreach (var kvp in groupEdges)
                     _groupEdges.TryRemove(kvp.Key, out ConnectionEdge _);
               }

               _graphSubject.OnNext(graph);
            }

            // Keep disconnected connection Ids for a while to prevent out of sequence messages from being registered.
            _disconnectionCache.GetOrCreate(connectionId, entry =>
            {
               entry.SetSlidingExpiration(TimeSpan.FromMinutes(10));
               entry.RegisterPostEvictionCallback((entryKey, value, reason, substate) =>
               {
                  _connectionsById.TryRemove((string) entryKey, out ConnectionInfo _);
               });
               return connectionId;
            });
         }
      }

      /// <summary>
      /// Receives a telemetry data from a hub.
      /// </summary>
      /// <param name="connectionId">Connection ID of the hub.</param>
      /// <param name="metrics">Hub metrics info.</param>
      public void ReceiveTelemetry(string hubId, string name, Dictionary<string, object> metrics)
      {
         var vertex = _verticesById.Values.FirstOrDefault(x => x.Id == hubId);
         if (vertex == null)
         {
            vertex = GetOrAddVertex(ConnectionVertexType.Hub, hubId, name, null, out bool _);
            _graphSubject.OnNext(new ConnectionGraph(vertex));
         }

         var graph = GetConnectionGraph();
         var totalClients = graph.Edges.Where(x => x.TargetId == vertex.Id).Count();

         metrics[$"{MetricsType.total_clients}"] = totalClients;
         metrics[$"{MetricsType.in_throughput}"] = vertex.InboundThroughput;
         metrics[$"{MetricsType.out_throughput}"] = vertex.OutboundThroughput;

         vertex.Metrics = metrics;
         _telemetrySubject.OnNext(new TelemetryData(vertex.Id, vertex.Name, metrics));
      }

      /// <summary>
      /// Clears stored information.
      /// </summary>
      public void Reset()
      {
         _connectionsById.Clear();
         _verticesById.Clear();
         _edgesById.Clear();
         _groupEdges.Clear();
      }

      private ConnectionInfo AddOrUpdateConnectionInfo(ConnectionContext context, DotNetifyHubContext hubContext)
      {
         var timeStamp = context.GetOriginContext()?.TimeStamp ?? context.TimeStamp;
         var result = _connectionsById.AddOrUpdate(context.ConnectionId, new ConnectionInfo
         {
            Context = context,
            OriginContext = context.GetOriginContext(),
            TimeStamp = timeStamp,
            CallType = hubContext.CallType,
            VMId = hubContext.VMId,
            Data = hubContext.Data,
            Principal = hubContext.Principal,
            GroupSend = hubContext.CallerContext.GetGroupSend()
         }, (id, info) =>
         {
            if (info.Context.TimeStamp < context.TimeStamp)
            {
               info.Context = context;
               info.OriginContext = context.GetOriginContext();
               info.TimeStamp = context.TimeStamp;
               info.CallType = hubContext.CallType;
               info.VMId = hubContext.VMId;
               info.Data = hubContext.Data;
               info.Principal = hubContext.Principal;
               info.GroupSend = hubContext.CallerContext.GetGroupSend();
            }
            return info;
         });

         _connectionSubject.OnNext(result);
         return result;
      }

      private ConnectionVertex GetOrAddVertex(ConnectionVertexType type, string id, string name, ConnectionContext context, out bool isNewVertex)
      {
         var timeStamp = DateTime.UtcNow;
         var vertex = _verticesById.GetOrAdd(id, vertexId =>
         {
            return new ConnectionVertex
            {
               Id = vertexId,
               Type = type,
               Name = name,
               Context = context,
               CreatedTime = timeStamp
            };
         });

         isNewVertex = timeStamp == vertex.CreatedTime;

         if (vertex.Name != name)
         {
            vertex.Name = name;
            vertex.Context = context;
            isNewVertex = true;
         }

         return vertex;
      }

      private ConnectionEdge GetOrAddEdge(string id, string sourceId, string targetId, ConnectionInfo info, out bool isNewEdge)
      {
         var timeStamp = DateTime.UtcNow;
         var result = _edgesById.GetOrAdd(id, edgeId =>
         {
            var edge = new ConnectionEdge
            {
               Id = edgeId,
               SourceId = sourceId,
               TargetId = targetId,
               Label = info.Context.ConnectionId,
               Info = info,
               CreatedTime = timeStamp
            };

            if (info.CallType == nameof(IDotNetifyHubMethod.Response_VM))
               edge.Inbound = new ConnectionEdge.Message(info.Data, info.TimeStamp);
            else
               edge.Outbound = new ConnectionEdge.Message(info.Data, info.TimeStamp);
            return edge;
         });

         isNewEdge = timeStamp == result.CreatedTime;
         return result;
      }

      private IEnumerable<string> GetGroupConnections(VMController.GroupSend groupSend)
      {
         if (groupSend == null)
            return null;

         if (!string.IsNullOrEmpty(groupSend.GroupName))
            return _verticesById.Where(x => x.Value.GroupName == groupSend.GroupName && !groupSend.ExcludedConnectionIds.Contains(x.Key)).Select(x => x.Value.Id);
         else
            return groupSend.ConnectionIds.Except(groupSend.ExcludedConnectionIds);
      }

      private void Log(ConnectionInfo info, DotNetifyHubContext hubContext, ConnectionVertex sourceVertex, ConnectionVertex targetVertex)
      {
         if (_logger.IsEnabled(LogLevel.Trace))
         {
            var message = new
            {
               ConnId = info.Context.ConnectionId,
               OriginConnId = info.OriginContext?.ConnectionId,
               Src = sourceVertex.Name,
               SrcId = sourceVertex.Id,
               Target = targetVertex.Name,
               hubContext.CallType,
               hubContext.Data,
               GroupSend = hubContext.CallerContext.GetGroupSend()
            };
            _logger.LogTrace(message.SerializeToText());
         }
      }

      private void UpdateEdgeInfo(ConnectionEdge edge, ConnectionInfo info)
      {
         lock (edge)
         {
            edge.Info.VMId = info.VMId;
            edge.Info.CallType = info.CallType;
            edge.Info.Data = info.Data;
            edge.Info.TimeStamp = info.TimeStamp;

            if (info.CallType == nameof(IDotNetifyHubMethod.Response_VM))
               edge.Inbound = new ConnectionEdge.Message(info.Data, info.TimeStamp);
            else
               edge.Outbound = new ConnectionEdge.Message(info.Data, info.TimeStamp);
         }
      }

      private void UpdateGroupEdgeInfo(VMController.GroupSend groupSend, ConnectionInfo info)
      {
         foreach (var connectionId in GetGroupConnections(groupSend))
         {
            if (_edgesById.TryGetValue(connectionId, out ConnectionEdge edge))
               UpdateEdgeInfo(edge, info);
         }
      }

      private void UpdateThroughput(ConnectionInfo info, DotNetifyHubContext hubContext, ConnectionVertex sourceVertex, ConnectionVertex targetVertex)
      {
         if (info.CallType == nameof(IDotNetifyHubMethod.Response_VM))
         {
            var groupSend = hubContext.CallerContext.GetGroupSend();
            var groupSendConnections = GetGroupConnections(groupSend);

            if (groupSend != null)
            {
               UpdateGroupEdgeInfo(groupSend, info);

               foreach (var connectionId in groupSendConnections)
                  if (_verticesById.TryGetValue(connectionId, out ConnectionVertex clientVertex))
                     clientVertex.IncrementInbound();
            }
            else
            {
               if (info.OriginContext != null)
               {
                  if (_verticesById.TryGetValue(info.OriginContext.ConnectionId, out ConnectionVertex clientVertex))
                  {
                     clientVertex.IncrementInbound();

                     // Response to a multicast VM request will have a group name.
                     SetGroupName(clientVertex, hubContext.CallerContext.GetGroupName());
                  }
               }
               else
               {
                  sourceVertex.IncrementInbound();

                  // Response to a multicast VM request will have a group name.
                  SetGroupName(sourceVertex, hubContext.CallerContext.GetGroupName());
               }
            }

            // The origin context of a response is the connection context of the forwarding hub.
            if (info.OriginContext != null)
            {
               if (_edgesById.TryGetValue(info.OriginContext.ConnectionId, out ConnectionEdge clientEdge))
                  UpdateEdgeInfo(clientEdge, info);

               targetVertex.IncrementOutbound();

               if (groupSendConnections?.Count() > 0)
               {
                  // Forwarded group message is reported only one of the hub forwarder, so we need to look by connection Ids
                  // to correctly compute the throughputs.
                  var hubVertices = new List<ConnectionVertex>();
                  foreach (var connectionId in groupSendConnections)
                  {
                     var hubId = _edgesById.FirstOrDefault(x => x.Value.SourceId == connectionId).Value?.TargetId;
                     if (hubId != null && _verticesById.TryGetValue(hubId, out ConnectionVertex hubVertex))
                     {
                        hubVertex.IncrementOutbound();
                        if (!hubVertices.Contains(hubVertex))
                        {
                           hubVertices.Add(hubVertex);
                           hubVertex.IncrementInbound();
                        }
                     }
                  }
               }
               else
                  sourceVertex.IncrementInbound();
            }
            else
               targetVertex.IncrementOutbound(groupSendConnections?.Count() ?? 1);
         }
         else
         {
            sourceVertex.IncrementOutbound();
            targetVertex.IncrementInbound();
         }
      }

      private void SetGroupName(ConnectionVertex clientVertex, string groupName)
      {
         if (string.IsNullOrWhiteSpace(groupName) || clientVertex.GroupName == groupName)
            return;

         clientVertex.GroupName = groupName;

         var edge = _edgesById.FirstOrDefault(x => x.Value.SourceId == clientVertex.Id);
         var graph = new ConnectionGraph(clientVertex, edge.Value);

         foreach (var kvp in _verticesById.Where(x => x.Value.GroupName == groupName))
         {
            string groupEdgeId = $"{groupName}.{clientVertex.Id}.{kvp.Key}";
            var groupEdge = new ConnectionEdge { Id = groupEdgeId, SourceId = clientVertex.Id, TargetId = kvp.Key, Label = "$group" };
            graph.Edges.Add(groupEdge);
            _groupEdges.TryAdd(groupEdgeId, groupEdge);
         }

         _graphSubject.OnNext(graph);
      }
   }
}
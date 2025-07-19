using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace DotNetify.Observer
{
   /// <summary>
   /// Displays information on a selected connection node, which can be a hub or a client connected to the hub.
   /// </summary>
   public class NodeInfoVM : BaseVM
   {
      private static readonly TimeSpan UPDATE_INTERVAL = TimeSpan.FromMilliseconds(500);

      private readonly IConnectionTracker _connectionTracker;
      private readonly INodeInfoBuilder _nodeInfoBuilder;
      private List<IDisposable> _subs = new List<IDisposable>();
      private ConnectionVertex _selectedVertex;
      private DateTime _lastUpdate;

      public IList<NodeInfoItem> InfoItems
      {
         get => Get<IList<NodeInfoItem>>();
         set => Set(value);
      }

      public NodeInfoVM(IAppState appState, IConnectionTracker connectionTracker, INodeInfoBuilder nodeInfoBuilder)
      {
         _connectionTracker = connectionTracker;
         _nodeInfoBuilder = nodeInfoBuilder;

         // If a node selected on the UI app, display that node's info.
         _subs.Add(appState.SelectedNodeId.Subscribe(id => OnSelectNodeId(id)));

         // If an incoming connection message is associated with the selected node, update that node's info.
         _subs.Add(connectionTracker.Connection
            .Subscribe(info =>
         {
            if (_selectedVertex?.Type == ConnectionVertexType.Client)
            {
               var connectionId = info.OriginContext?.ConnectionId ?? info.Context.ConnectionId;
               if (_selectedVertex.Context.ConnectionId == connectionId || info.InMulticast(_selectedVertex.Context.ConnectionId))
               {
                  OnSelectNodeId(_selectedVertex.Id);
                  PushUpdates();
               }
            }
         }));

         // Update node's info regularly since the client UI displays info relative to current time.
         _subs.Add(Observable.Interval(UPDATE_INTERVAL).Subscribe(_ =>
         {
            if (_selectedVertex != null && (DateTime.UtcNow - _lastUpdate) > UPDATE_INTERVAL)
            {
               OnSelectNodeId(_selectedVertex.Id);
               PushUpdates();
            }
         }));
      }

      public override void Dispose() => _subs.ForEach(x => x.Dispose());

      private void OnSelectNodeId(string nodeId)
      {
         var graph = _connectionTracker.GetConnectionGraph();
         _selectedVertex = graph.Vertices.ToList().Find(vertex => vertex.Id == nodeId);
         if (_selectedVertex != null)
         {
            if (_selectedVertex.Type == ConnectionVertexType.Client)
            {
               var edge = graph.Edges.ToList().FirstOrDefault(x => x.SourceId == nodeId);
               if (edge?.Info != null)
                  InfoItems = _nodeInfoBuilder.GetClientInfo(_selectedVertex, edge);
            }
            else if (_selectedVertex.Type == ConnectionVertexType.Hub)
            {
               InfoItems = _nodeInfoBuilder.GetHubInfo(_selectedVertex);
            }
         }
         else
            InfoItems = null;

         _lastUpdate = DateTime.UtcNow;
      }
   }
}
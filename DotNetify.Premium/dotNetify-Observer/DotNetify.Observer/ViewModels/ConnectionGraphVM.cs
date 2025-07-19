using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace DotNetify.Observer
{
   public class ConnectionGraphVM : BaseVM
   {
      private readonly IAppState _appState;
      private readonly List<IDisposable> _subs = new List<IDisposable>();

      public class Node
      {
         public string Id { get; set; }
         public string Name { get; set; }
         public int GroupHash { get; set; }
         public bool IsHub { get; set; }

         public Node(string id, string name, int groupHash, bool isHub)
         {
            Id = id;
            Name = name;
            GroupHash = groupHash;
            IsHub = isHub;
         }
      }

      public class Link
      {
         public string Id { get; set; }
         public string Source { get; set; }
         public string Target { get; set; }
         public string Label { get; set; }
         public bool IsGroup { get; set; }

         public Link(string id, string source, string target, string label, bool isGroup)
         {
            Id = id;
            Source = source;
            Target = target;
            Label = label;
            IsGroup = isGroup;
         }
      }

      public class Graph
      {
         public Node[] Nodes { get; set; }
         public Link[] Links { get; set; }
         public bool Removed { get; set; }

         public Graph(Node[] nodes, Link[] links, bool removed)
         {
            Nodes = nodes;
            Links = links;
            Removed = removed;
         }
      }

      public Graph GraphUpdate { get => Get<Graph>(); set => Set(value); }

      public string SelectedNodeId { set => _appState.SelectedNodeId.OnNext(value); }

      public ConnectionGraphVM(IConnectionTracker connectionTracker, IAppState appState)
      {
         License.Check();

         _appState = appState;

         var graph = connectionTracker.GetConnectionGraph();

         GraphUpdate = new Graph(
            graph.Vertices.Select(x => ToNode(x)).ToArray(),
            graph.Edges.Select(x => ToLink(x)).ToArray(),
            false
         );

         _subs.Add(connectionTracker.Graph.Subscribe(g =>
         {
            GraphUpdate = new Graph(
               g.Vertices.Select(x => ToNode(x)).ToArray(),
               g.Edges.Select(x => ToLink(x)).ToArray(),
               g.Removed
            );

            PushUpdates();
         }));
      }

      public override void Dispose() => _subs.ForEach(x => x.Dispose());

      private Link ToLink(ConnectionEdge edge) => new Link(edge.Id, edge.SourceId, edge.TargetId, edge.Label, edge.Label == "$group");

      private Node ToNode(ConnectionVertex vertex) => new Node(vertex.Id, vertex.Name, vertex.GroupName != null ? vertex.GroupName.GetHashCode() : 0, vertex.Type == ConnectionVertexType.Hub);
   }
}
using System.Collections.Generic;

namespace DotNetify.Observer
{
   public class ConnectionGraph
   {
      public IList<ConnectionVertex> Vertices { get; set; } = new List<ConnectionVertex>();
      public IList<ConnectionEdge> Edges { get; set; } = new List<ConnectionEdge>();
      public bool Removed { get; set; }

      public ConnectionGraph()
      {
      }

      public ConnectionGraph(ConnectionVertex vertex)
      {
         Vertices.Add(vertex);
      }

      public ConnectionGraph(ConnectionVertex vertex, ConnectionEdge edge) : this(vertex)
      {
         Edges.Add(edge);
      }
   }
}
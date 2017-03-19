using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphX.PCL.Common.Models;
using QuickGraph;

namespace Graphs
{
    class GraphXVertex<T> : VertexBase
    {
        public Vertex<T> Vertex { get; }

        public string Name => Vertex.Name;
        public T Data => Vertex.Data;

        public GraphXVertex(Vertex<T> vertex)
        {
            Vertex = vertex;
        }

        public override string ToString()
        {
            return $"{Name}: {Data}";
        }
    }

    class GraphXEdge<T> : EdgeBase<GraphXVertex<T>>
    {
        public GraphXEdge(IEdge<Vertex<T>> edge, Dictionary<string, GraphXVertex<T>> vertexDict) : base(vertexDict[edge.Source.Name], vertexDict[edge.Target.Name])
        {
            var taggedEdge = edge as ITagged<double>;
            if (taggedEdge != null)
                Weight = taggedEdge.Tag;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Graphs
{
    class Graph<TVertex>
        where TVertex: VertexBase
    {
        private Dictionary<string, TVertex> _nameVertexDictionary;
        private MultiValueDictionary<TVertex, EdgeBase<TVertex>> _vertexEdgeDictionary;
        private List<EdgeBase<TVertex>> _edgeList;

        public IReadOnlyCollection<EdgeBase<TVertex>> EdgeList => _edgeList;
        public IReadOnlyDictionary<string, TVertex> NameVertexDictionary => _nameVertexDictionary;
        public ILookup<TVertex, EdgeBase<TVertex>> VertexEdgeLookup => _vertexEdgeDictionary.AsLookup();

        public bool IsDirected { get; }

        public Graph(bool isDirected = false)
        {
            _nameVertexDictionary = new Dictionary<string, TVertex>();
            _vertexEdgeDictionary = new MultiValueDictionary<TVertex, EdgeBase<TVertex>>();
            _edgeList = new List<EdgeBase<TVertex>>();

            IsDirected = isDirected;
        }

        public void AddVertex(TVertex vertex)
        {
            _nameVertexDictionary[vertex.Name] = vertex;
        }

        public void AddEdge(EdgeBase<TVertex> edge)
        {
            if (_nameVertexDictionary.TryGetValue(edge.Source.Name, out TVertex origSourceVertex) && origSourceVertex != edge.Source)
                throw new InvalidDataException($"Vertex '{edge.Source.Name}' already exists but is diffrent than the given one.");
            if (_nameVertexDictionary.TryGetValue(edge.Target.Name, out TVertex origTargetVertex) && origTargetVertex != edge.Target)
                throw new InvalidDataException($"Vertex '{edge.Source.Name}' already exists but is diffrent than the given one.");

            _nameVertexDictionary[edge.Source.Name] = edge.Source;
            _nameVertexDictionary[edge.Target.Name] = edge.Target;

            _vertexEdgeDictionary.Add(edge.Source, edge);
            _vertexEdgeDictionary.Add(edge.Target, edge);

            _edgeList.Add(edge);
        }

        public void AddEdge(TVertex source, TVertex target)
        {
            AddEdge(new EdgeBase<TVertex>(source, target));
        }

        public void AddEdge(TVertex source, TVertex target, double? weight)
        {
            AddEdge(new EdgeBase<TVertex>(source, target, weight));
        }

        public void RemoveVertex(string vertexName)
        {
            if (_nameVertexDictionary.TryGetValue(vertexName, out TVertex vertex))
            {
                _nameVertexDictionary.Remove(vertexName);

                if (_vertexEdgeDictionary.TryGetValue(vertex, out IReadOnlyCollection<EdgeBase<TVertex>> vertexEdges))
                {
                    foreach (var vertexEdge in vertexEdges)
                    {
                        TVertex otherVertex;
                        if (vertexEdge.Source != vertex)
                            otherVertex = vertexEdge.Source;
                        else if (vertexEdge.Target != vertex)
                            otherVertex = vertexEdge.Target;
                        else
                            continue;
                            
                        _vertexEdgeDictionary.Remove(otherVertex, vertexEdge);
                        _edgeList.Remove(vertexEdge);
                    }
                    _vertexEdgeDictionary.Remove(vertex);
                }
            }
        }

        public void RemoveVertex(TVertex vertex)
        {
            RemoveVertex(vertex.Name);
        }

        public void RemoveEdge(EdgeBase<TVertex> edge)
        {
            _vertexEdgeDictionary.Remove(edge.Source, edge);
            _vertexEdgeDictionary.Remove(edge.Target, edge);
            _edgeList.Remove(edge);
        }
    }

    class VertexBase
    {
        public string Name { get; set; }

        public string Data { get; set; }

        public override string ToString()
        {
            return $"Vertex{{Name={Name}, Data={Data}}}";
        }
    }

    class EdgeBase<TVertex>
        where TVertex: VertexBase
    {
        public TVertex Source { get; }
        public TVertex Target { get; }
        public double? Weight { get; set; }

        public EdgeBase(TVertex source, TVertex target, double? weight = null)
        {
            Source = source;
            Target = target;
            Weight = weight;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using Graphs.Annotations;
using Graphs.Utils;

namespace Graphs
{
    partial class Graph<TVertex>
        where TVertex: VertexBase
    {
        private Dictionary<string, TVertex> _nameVertexDictionary;
        private MultiValueDictionary<TVertex, EdgeBase<TVertex>> _vertexEdgeDictionary;
        private List<EdgeBase<TVertex>> _edgeList;

        public IEnumerable<TVertex> Vertices => NameVertexDictionary.Values;
        public IReadOnlyCollection<EdgeBase<TVertex>> Edges => _edgeList;
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

        public IEnumerable<(TVertex vertex, EdgeBase<TVertex> edge)> GetNeighborsWithEdges(TVertex vertex, bool ignoreSelfLoops = false)
        {
            foreach (var edge in _vertexEdgeDictionary[vertex])
            {
                if (edge.Source != vertex)
                {
                    if (IsDirected)
                        continue;
                    yield return (edge.Source, edge);
                }
                else if (edge.Target != vertex)
                    yield return (edge.Target, edge);
                else if (!ignoreSelfLoops)
                    yield return (vertex, edge);
            }
        }
        public IEnumerable<TVertex> GetNeighbors(TVertex vertex, bool ignoreSelfLoops = false)
        {
            return GetNeighborsWithEdges(vertex, ignoreSelfLoops).Select(vertexEdgeTuple => vertexEdgeTuple.vertex);
        }

        public int GetDegree(TVertex vertex)
        {
            return _vertexEdgeDictionary[vertex].Sum(edge => edge.Source == edge.Target ? 2 : 1);
        }

        public int GetOutDegree(TVertex vertex)
        {
            return _vertexEdgeDictionary[vertex].Sum(edge => edge.Source == vertex ? 1 : 0);
        }

        public int GetInDegree(TVertex vertex)
        {
            return _vertexEdgeDictionary[vertex].Sum(edge => edge.Target == vertex ? 1 : 0);
        }

        public void BreadthFirstSearch([NotNull] TVertex startVertex, Predicate<TVertex> visitorAction)
        {
            Queue<TVertex> queue = new Queue<TVertex>(new []{startVertex});
            HashSet<TVertex> visited = new HashSet<TVertex>(queue);
            while (!queue.IsEmpty())
            {
                var curVertex = queue.Dequeue();
                if (visitorAction(curVertex)) break;
                var neighbors = GetNeighbors(curVertex).Distinct().Except(visited).ToArray();
                queue.EnqueueRange(neighbors);
                visited.AddRange(neighbors);
            }
        }

        public void DepthFirstSearch([NotNull] TVertex startVertex, Predicate<TVertex> visitorAction)
        {
            Stack<TVertex> stack = new Stack<TVertex>(new[] { startVertex });
            HashSet<TVertex> visited = new HashSet<TVertex>();
            while (!stack.IsEmpty())
            {
                var curVertex = stack.Pop();
                if (!visited.Contains(curVertex))
                {
                    if (visitorAction(curVertex)) break;
                    visited.Add(curVertex);
                    stack.PushRange(GetNeighbors(curVertex).Distinct().Except(visited));
                }
            }
        }

        public bool IsConnected()
        {
            var allVertices = Vertices.ToHashSet();
            BreadthFirstSearch(allVertices.First(), vertex =>
            {
                allVertices.Remove(vertex);
                return false;
            });
            return allVertices.IsEmpty();
        }

        public void ResetColoring()
        {
            IsAnimationPlaying = false;
            foreach (var vertex in Vertices)
            {
                vertex.ResetColor();
            }
            foreach (var edge in Edges)
            {
                edge.ResetColor();
            }
        }
    }

    class VertexBase : PropertyChangedBase
    {
        public static Brush DefaultForegroundBrush { get; } = Brushes.Black;
        public static Brush DefaultBackgroundBrush { get; } = Brushes.LightGray;
        public static Brush SelectedForegroundBrush { get; } = Brushes.White;
        public static Brush SelectedBackgroundBrush { get; } = Brushes.DimGray;

        public string Name { get; set; }
        public string Data { get; set; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; ResetColor(); }
        }

        public Brush ForegroundBrush
        {
            get { return _foregroundBrush; }
            set { _foregroundBrush = value; OnNotifyPropertyChanged(); }
        }

        public Brush BackgroundBrush
        {
            get { return _backgroundBrush; }
            set { _backgroundBrush = value; OnNotifyPropertyChanged(); }
        }

        private Brush _foregroundBrush = DefaultForegroundBrush;
        private Brush _backgroundBrush = DefaultBackgroundBrush;
        private bool _isSelected;

        public void ResetColor()
        {
            if (IsSelected)
            {
                ForegroundBrush = SelectedForegroundBrush;
                BackgroundBrush = SelectedBackgroundBrush;
            }
            else
            {
                ForegroundBrush = DefaultForegroundBrush;
                BackgroundBrush = DefaultBackgroundBrush;
            }
        }

        public override string ToString()
        {
            return $"Vertex{{Name={Name}, Data={Data}}}";
        }
    }

    class EdgeBase<TVertex> : PropertyChangedBase
        where TVertex: VertexBase
    {
        public static Brush DefaultStrokeBrush { get; } = Brushes.Black;

        public TVertex Source { get; }
        public TVertex Target { get; }
        public double? Weight { get; set; }

        public Brush StrokeBrush
        {
            get { return _strokeBrush; }
            set { _strokeBrush = value; OnNotifyPropertyChanged(); }
        }

        private Brush _strokeBrush = DefaultStrokeBrush;

        public EdgeBase(TVertex source, TVertex target, double? weight = null)
        {
            Source = source;
            Target = target;
            Weight = weight;
        }

        public void ResetColor()
        {
            StrokeBrush = DefaultStrokeBrush;
        }
    }
}

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
using MoreLinq;

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

        public Graph(Graph<TVertex> oldGraph, bool? isDirected = null)
        {
            _nameVertexDictionary = new Dictionary<string, TVertex>(oldGraph._nameVertexDictionary);
            _vertexEdgeDictionary = new MultiValueDictionary<TVertex, EdgeBase<TVertex>>(oldGraph._vertexEdgeDictionary);
            _edgeList = new List<EdgeBase<TVertex>>(oldGraph._edgeList);

            IsDirected = isDirected ?? oldGraph.IsDirected;
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
            if (edge.Source != edge.Target)
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

        public IEnumerable<EdgeBase<TVertex>> GetEdges(TVertex source, TVertex target)
        {
            var edgeEnumerable = _vertexEdgeDictionary[source].AsEnumerable().Where(edge => edge.Source == source && edge.Target == target);
            if (!IsDirected) edgeEnumerable = edgeEnumerable.Where(edge => edge.Target == source && edge.Source == target);
            return edgeEnumerable;
        }

        public EdgeBase<TVertex> GetEdge(TVertex source, TVertex target)
        {
            return GetEdges(source, target).FirstOrDefault();
        }

        public IEnumerable<(TVertex Vertex, EdgeBase<TVertex> Edge)> GetNeighborsWithEdges(TVertex vertex, bool ignoreSelfLoops = false)
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
            return GetNeighborsWithEdges(vertex, ignoreSelfLoops).Select(vertexEdgeTuple => vertexEdgeTuple.Vertex);
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

        public IEnumerable<TVertex> BreadthFirstSearch([NotNull] TVertex startVertex)
        {
            Queue<TVertex> queue = new Queue<TVertex>(new []{startVertex});
            HashSet<TVertex> visited = new HashSet<TVertex>(queue);
            while (!queue.IsEmpty())
            {
                var curVertex = queue.Dequeue();
                yield return curVertex;
                var neighbors = GetNeighbors(curVertex).Distinct().Except(visited).ToArray();
                queue.EnqueueRange(neighbors);
                visited.AddRange(neighbors);
            }
        }

        public IEnumerable<TVertex> DepthFirstSearch([NotNull] TVertex startVertex)
        {
            Stack<TVertex> stack = new Stack<TVertex>(new[] { startVertex });
            HashSet<TVertex> visited = new HashSet<TVertex>();
            while (!stack.IsEmpty())
            {
                var curVertex = stack.Pop();
                if (!visited.Contains(curVertex))
                {
                    yield return curVertex;
                    visited.Add(curVertex);
                    stack.PushRange(GetNeighbors(curVertex).Distinct().Except(visited));
                }
            }
        }

        public bool IsConnected()
        {
            return !Vertices.Except(BreadthFirstSearch(Vertices.First())).Any();
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
        public static Brush DefaultBorderBrush { get; } = Brushes.DarkGray;
        public static Brush SelectedBorderBrush { get; } = Brushes.Black;

        public string Name { get; }

        public string Data
        {
            get { return _data; }
            set {
                _data = value;
                OnNotifyPropertyChanged();
                OnNotifyPropertyChanged(nameof(Label));
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnNotifyPropertyChanged(); }
        }

        public Brush ForegroundBrush
        {
            get => _foregroundBrush;
            set { _foregroundBrush = value; OnNotifyPropertyChanged(); }
        }

        public Brush BackgroundBrush
        {
            get => _backgroundBrush;
            set { _backgroundBrush = value; OnNotifyPropertyChanged(); }
        }

        public string Label
        {
            get
            {
                string result = Name;
                if (Data != null)
                    result = $"{result}: {Data}";
                return result;
            }
        }

        private Brush _foregroundBrush = DefaultForegroundBrush;
        private Brush _backgroundBrush = DefaultBackgroundBrush;
        private bool _isSelected;
        private string _data;

        public VertexBase(string name, string data = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Data = data;
            _isSelected = false;
        }

        public void ResetColor()
        {
            ForegroundBrush = DefaultForegroundBrush;
            BackgroundBrush = DefaultBackgroundBrush;
        }

        public bool IsInEdge(EdgeBase<VertexBase> edge)
        {
            return this == edge.Target;
        }

        public bool IsOutEdge(EdgeBase<VertexBase> edge)
        {
            return this == edge.Source;
        }

        public VertexBase GetOtherVertex(EdgeBase<VertexBase> edge)
        {
            return this == edge.Source ? edge.Target : (this == edge.Target ? edge.Source : null);
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
            get => _strokeBrush;
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

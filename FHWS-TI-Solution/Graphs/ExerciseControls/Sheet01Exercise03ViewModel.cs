using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Graphs.Utils;

namespace Graphs.ExerciseControls
{
    class Sheet01Exercise03ViewModel : ExerciseViewModelBase
    {
        public IEnumerable<string> VertexNames => _graph?.Vertices.Select(vertex => vertex.Name);

        public string StartVertexName { get; set; }
        public string EndVertexName { get; set; }

        public RelayCommand RunDijkstraCommand { get; }

        public Sheet01Exercise03ViewModel()
        {
            RunDijkstraCommand = new RelayCommand(() =>
            {
                if (_graph != null && StartVertexName != null && EndVertexName != null &&
                    _graph.NameVertexDictionary.TryGetValue(StartVertexName, out VertexBase startVertex) &&
                    _graph.NameVertexDictionary.TryGetValue(EndVertexName, out VertexBase endVertex))
                {
                    _graph.ResetColoring();
                    var shortestPath = _graph.FindShortestPathWithDijkstra(startVertex, endVertex);
                    if (shortestPath == null)
                        return;
                    foreach (var vertexAndEdge in shortestPath)
                    {
                        vertexAndEdge.Vertex.BackgroundBrush = Brushes.LimeGreen;
                        if (vertexAndEdge.EdgeToParent != null)
                            vertexAndEdge.EdgeToParent.StrokeBrush = Brushes.LimeGreen;
                    }
                }
            });
        }

        protected override void OnGraphUpdated()
        {
            OnNotifyPropertyChanged(nameof(VertexNames));
        }

    }
}

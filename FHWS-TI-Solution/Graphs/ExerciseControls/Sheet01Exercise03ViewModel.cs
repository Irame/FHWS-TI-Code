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

        public RelayCommand<double> RunDijkstraCommand { get; }

        public Sheet01Exercise03ViewModel()
        {
            RunDijkstraCommand = new RelayCommand<double>(speed =>
            {
                if (_graph != null && StartVertexName != null && EndVertexName != null &&
                    _graph.NameVertexDictionary.TryGetValue(StartVertexName, out VertexBase startVertex) &&
                    _graph.NameVertexDictionary.TryGetValue(EndVertexName, out VertexBase endVertex))
                {
                    _graph.WalkThroughDijkstra(startVertex, endVertex, (int)speed);
                }
            });
        }

        protected override void OnGraphUpdated()
        {
            OnNotifyPropertyChanged(nameof(VertexNames));
        }

    }
}

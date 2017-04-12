using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Graphs.Utils;

namespace Graphs.ExerciseControls
{
    class Sheet01Exercise04ViewModel : ExerciseViewModelBase
    {
        public IEnumerable<string> VertexNames => _graph?.Vertices.Select(vertex => vertex.Name);

        public string StartVertexName { get; set; }
        public string EndVertexName { get; set; }

        public RelayCommand RunFordFulkersonCommand { get; }

        public double MaximumFlow
        {
            get => _maximumFlow;
            private set { _maximumFlow = value; OnNotifyPropertyChanged(); }
        }

        private double _maximumFlow;

        public Sheet01Exercise04ViewModel()
        {
            MaximumFlow = 0;

            RunFordFulkersonCommand = new RelayCommand(() =>
            {
                if (_graph.NameVertexDictionary.TryGetValue(StartVertexName, out VertexBase startVertex) &&
                    _graph.NameVertexDictionary.TryGetValue(EndVertexName, out VertexBase endVertex))
                {
                    MaximumFlow = _graph.GetGreatestFlowWithFordFulkerson(startVertex, endVertex);
                }
            }, () => _graph != null && StartVertexName != null && EndVertexName != null);
        }

        protected override void OnGraphUpdated()
        {
            OnNotifyPropertyChanged(nameof(VertexNames));
        }

    }
}

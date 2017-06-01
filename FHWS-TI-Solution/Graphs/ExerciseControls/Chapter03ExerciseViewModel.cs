using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Graphs.Utils;

namespace Graphs.ExerciseControls
{
    class Chapter03ExerciseViewModel : ExerciseViewModelBase
    {
        public RelayCommand NearestInsertionCommand { get; }
        public RelayCommand FarestInsertionCommand { get; }
        public RelayCommand RandomInsertionCommand { get; }

        private double _circuitLength;
        public double CircuitLength
        {
            get => _circuitLength;
            private set { _circuitLength = value; OnNotifyPropertyChanged(); }
        }

        public Chapter03ExerciseViewModel()
        {
            NearestInsertionCommand = new RelayCommand(() =>
            {
                _graph.ResetColoring();
                List<VertexBase> C = _graph.NearestInsertion();
                double lenght = 0;
                for (int i = 0; i < C.Count; i++)
                {
                    var edge = _graph.GetEdge(C[i], C[(i + 1) % C.Count]);
                    edge.StrokeBrush = Brushes.BlueViolet;
                    lenght += edge.Weight ?? 1;
                }
                CircuitLength = lenght;
            }, () => _graph != null);

            FarestInsertionCommand = new RelayCommand(() =>
            {
                _graph.ResetColoring();
                List<VertexBase> C = _graph.FarestInsertion();
                double lenght = 0;
                for (int i = 0; i < C.Count; i++)
                {
                    var edge = _graph.GetEdge(C[i], C[(i + 1) % C.Count]);
                    edge.StrokeBrush = Brushes.Blue;
                    lenght += edge.Weight ?? 1;
                }
                CircuitLength = lenght;
            }, () => _graph != null);

            RandomInsertionCommand = new RelayCommand(() =>
            {
                _graph.ResetColoring();
                List<VertexBase> C = _graph.RandomInsertion();
                double lenght = 0;
                for (int i = 0; i < C.Count; i++)
                {
                    var edge = _graph.GetEdge(C[i], C[(i + 1) % C.Count]);
                    edge.StrokeBrush = Brushes.DarkRed;
                    lenght += edge.Weight ?? 1;
                }
                CircuitLength = lenght;
            }, () => _graph != null);
        }

        protected override void OnGraphUpdated()
        {
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphs.Utils;

namespace Graphs.ExerciseControls
{
    class Sheet01Exercise02ViewModel : ExerciseViewModelBase
    {
        private Graph<VertexBase> _graph;

        public ValueWraper<bool?> EulerianPathCheckResult { get; } = new ValueWraper<bool?>(false);
        public ValueWraper<bool?> EulerianCircuitCheckResult { get; } = new ValueWraper<bool?>(false);

        public RelayCommand CheckForEulerianPathsCommand { get; }
        public RelayCommand CheckForEulerianCircuitsCommand { get; }

        public Sheet01Exercise02ViewModel()
        {
            CheckForEulerianPathsCommand = new RelayCommand(() =>
            {
                EulerianPathCheckResult.Value = _graph?.HasEulerianPath();
            });

            CheckForEulerianCircuitsCommand = new RelayCommand(() =>
            {
                EulerianCircuitCheckResult.Value = _graph?.HasEulerianCircuit();
            });
        }

        public override void UpdateGraph(Graph<VertexBase> graph)
        {
            _graph = graph;
            EulerianPathCheckResult.Value = null;
            EulerianCircuitCheckResult.Value = null;
        }

    }
}

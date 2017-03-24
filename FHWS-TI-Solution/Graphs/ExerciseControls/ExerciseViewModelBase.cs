using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphs.Utils;

namespace Graphs.ExerciseControls
{
    abstract class ExerciseViewModelBase : ViewModelBase
    {
        protected Graph<VertexBase> _graph;

        public void UpdateGraph(Graph<VertexBase> graph)
        {
            _graph = graph;
            OnGraphUpdated();
        }

        protected abstract void OnGraphUpdated();
    }
}

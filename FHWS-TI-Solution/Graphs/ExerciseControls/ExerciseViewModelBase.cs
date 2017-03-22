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
        public abstract void UpdateGraph(Graph<VertexBase> graph);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphs.Utils;

namespace Graphs.ExerciseControls
{
    class Chapter01ExerciseViewModel : ExerciseViewModelBase
    {
        public RelayCommand<bool> GreedyColCommand { get; }

        public Chapter01ExerciseViewModel()
        {
            GreedyColCommand = new RelayCommand<bool>(shuffle =>
            {
                _graph.GreedyCol(shuffle);
            }, _ => _graph != null);
        }

        protected override void OnGraphUpdated()
        {
            
        }
    }
}

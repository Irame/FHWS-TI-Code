using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Graphs
{
    partial class Graph<TVertex>
    {
        public bool IsAnimationPlaying { get; private set; } = false;

        public void WalkThroughBreadthFirstSearch(int speed = 500)
        {
            if (IsAnimationPlaying) return;

            IsAnimationPlaying = true;
            Task.Run(() =>
            {
                BreadthFirstSearch(Vertices.First(), vertex =>
                {
                    Task.Delay(speed).Wait();
                    vertex.BackgroundBrush = Brushes.LimeGreen;
                });
            }).ContinueWith(task => IsAnimationPlaying = false);
        }
    }
}

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
            if (IsAnimationPlaying || _nameVertexDictionary.Count == 0)
                return;

            IsAnimationPlaying = true;
            Task.Run(() =>
            {
                BreadthFirstSearch(Vertices.FirstOrDefault(vertex => vertex.IsSelected) ?? Vertices.First(), vertex =>
                {
                    Task.Delay(speed).Wait();
                    if (IsAnimationPlaying)
                        vertex.BackgroundBrush = Brushes.LimeGreen;
                    return !IsAnimationPlaying;
                });
            }).ContinueWith(task => IsAnimationPlaying = false);
        }

        public void WalkThroughDepthFirstSearch(int speed = 500)
        {
            if (IsAnimationPlaying || _nameVertexDictionary.Count == 0)
                return;

            IsAnimationPlaying = true;
            Task.Run(() =>
            {
                DepthFirstSearch(Vertices.FirstOrDefault(vertex => vertex.IsSelected) ?? Vertices.First(), vertex =>
                {
                    Task.Delay(speed).Wait();
                    if (IsAnimationPlaying)
                        vertex.BackgroundBrush = Brushes.LimeGreen;
                    return !IsAnimationPlaying;
                });
            }).ContinueWith(task => IsAnimationPlaying = false);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Graphs.Utils;

namespace Graphs
{
    partial class Graph<TVertex>
    {
        public bool IsAnimationPlaying { get; private set; } = false;

        public async void WalkThroughBreadthFirstSearch(int speed = 500)
        {
            ResetColoring();
            await WalkThrough(
                BreadthFirstSearch(Vertices.FirstOrDefault(vertex => vertex.IsSelected) ?? Vertices.First()),
                vertex => vertex.BackgroundBrush = Brushes.LimeGreen, speed);
        }

        public async void WalkThroughDepthFirstSearch(int speed = 500)
        {
            ResetColoring();
            await WalkThrough(
                DepthFirstSearch(Vertices.FirstOrDefault(vertex => vertex.IsSelected) ?? Vertices.First()), 
                vertex => vertex.BackgroundBrush = Brushes.LimeGreen, speed);
        }

        public async void WalkThroughDijkstra(TVertex start, TVertex end, int speed = 500)
        {
            ResetColoring();
            await WalkThrough(FindShortestPathWithDijkstra(start, end), tuple =>
            {
                tuple.Vertex.BackgroundBrush = Brushes.LimeGreen;
                if (tuple.EdgeToParent != null)
                    tuple.EdgeToParent.StrokeBrush = Brushes.LimeGreen;
            }, speed);
        }

        public async Task WalkThrough<T>(IEnumerable<T> enumerable, Action<T> action, int speed = 500)
        {
            if (enumerable == null || IsAnimationPlaying || _nameVertexDictionary.Count == 0)
                return;

            IsAnimationPlaying = true;
            await enumerable.DelayedForEach(element =>
            {
                if (IsAnimationPlaying)
                    action(element);
                return IsAnimationPlaying;
            }, speed).ContinueWith(task => IsAnimationPlaying = false);
        }
    }
}

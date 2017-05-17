using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Graphs.Utils;
using MoreLinq;

namespace Graphs
{
    partial class Graph<TVertex> 
        where TVertex : VertexBase
    {
        public void GreedyCol(bool shuffle = false)
        {
            ResetColoring();

            var colors = new Brush[] { Brushes.LimeGreen, Brushes.Yellow, Brushes.Cyan, Brushes.Red, Brushes.Blue, Brushes.Green, Brushes.Purple};

            foreach (var vertex in shuffle ? Vertices.Shuffle() : Vertices)
            {
                vertex.BackgroundBrush = colors.Except(GetNeighbors(vertex).Select(v => v.BackgroundBrush)).First();
            }
        }
    }
}

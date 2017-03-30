using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;

namespace Graphs
{
    partial class Graph<TVertex>
    {
        public bool HasEulerianPath()
        {
            if (IsDirected)
            {
                return IsConnected() && Vertices.Select(vertex =>
                {
                    var degreeDiff = Math.Abs(GetInDegree(vertex) - GetOutDegree(vertex));
                    return degreeDiff < 2 ? degreeDiff : 3;
                }).TakeUntil(x => x == 3).Sum() == 2;
            }
            else
                return IsConnected() && Vertices.Sum(vertex => GetDegree(vertex) % 2) == 2;
        }

        public bool HasEulerianCircuit()
        {
            if (IsDirected)
                return IsConnected() && Vertices.All(vertex => GetInDegree(vertex) == GetOutDegree(vertex));
            else
                return IsConnected() && Vertices.All(vertex => GetDegree(vertex) % 2 == 0);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FibonacciHeap;
using Graphs.Utils;
using MoreLinq;
using QuickGraph;

namespace Graphs
{
    partial class Graph<TVertex>
    {
        private class DijkstraVertexInfo
        {
            public TVertex Vertex { get; }
            public DijkstraVertexInfo Parent { get; set; } = null;
            public EdgeBase<TVertex> ParentEdge { get; set; } = null;
            public double Distance { get; set; }

            public DijkstraVertexInfo(TVertex vertex, double distance)
            {
                Vertex = vertex;
                Distance = distance;
            }
        }

        public List<(TVertex Vertex, EdgeBase<TVertex> EdgeToParent)> FindShortestPathWithDijkstra(TVertex start, TVertex end)
        {
            var vertexInfoDict = Vertices.Select(vertex => new DijkstraVertexInfo(vertex, vertex == start ? 0 : double.PositiveInfinity))
                .Select(info => new FibonacciHeapNode<DijkstraVertexInfo>(info, info.Distance))
                .ToDictionary(node => node.Data.Vertex, node => node);

            var heap = new FibonacciHeap<DijkstraVertexInfo>();
            heap.InsertRange(vertexInfoDict.Values);


            DijkstraVertexInfo endInfo = null;
            while (vertexInfoDict.Count > 0)
            {
                var curVertexInfo = heap.RemoveMin().Data;
                if (curVertexInfo.Vertex == end)
                {
                    endInfo = curVertexInfo;
                    break;
                }
                
                var neighborsWithEdges = GetNeighborsWithEdges(curVertexInfo.Vertex, ignoreSelfLoops: true);
                foreach (var neighborsWithEdge in neighborsWithEdges)
                {
                    if (vertexInfoDict.TryGetValue(neighborsWithEdge.Vertex, out FibonacciHeapNode<DijkstraVertexInfo> neighborNode))
                    {
                        var alt = curVertexInfo.Distance + neighborsWithEdge.Edge.Weight ?? 1;
                        var neighborInfo = neighborNode.Data;
                        if (alt < neighborInfo.Distance)
                        {
                            heap.DecreaseKey(neighborNode, alt);
                            neighborInfo.Distance = alt;
                            neighborInfo.Parent = curVertexInfo;
                            neighborInfo.ParentEdge = neighborsWithEdge.Edge;
                        }
                    }
                }
            }

            if (endInfo != null)
            {
                var reslut = new List<(TVertex Vertex, EdgeBase<TVertex> EdgeToParent)>();
                var curVertex = endInfo;
                while (curVertex != null)
                {
                    reslut.Add((curVertex.Vertex, curVertex.ParentEdge));
                    curVertex = curVertex.Parent;
                }
                reslut.Reverse();
                return reslut;
            }

            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            public double Distance { get; set; }

            public DijkstraVertexInfo(TVertex vertex, double distance)
            {
                Vertex = vertex;
                Distance = distance;
            }
        }

        public List<TVertex> FindShortestPathWithDijkstra(TVertex start, TVertex end)
        {
            var vertexInfoDict = Vertices.Select(vertex => new DijkstraVertexInfo(vertex, vertex == start ? 0 : double.PositiveInfinity))
                .ToDictionary(vInfo => vInfo.Vertex, vInfo => vInfo);

            var endInfo = vertexInfoDict[end];
            while (vertexInfoDict.Count > 0)
            {
                var curVertexInfo = vertexInfoDict.Values.MinBy(info => info.Distance);
                if (curVertexInfo == endInfo)
                    break;

                vertexInfoDict.Remove(curVertexInfo.Vertex);
                var neighborsWithEdges = GetNeighborsWithEdges(curVertexInfo.Vertex, ignoreSelfLoops: true);
                foreach (var neighborsWithEdge in neighborsWithEdges)
                {
                    if (vertexInfoDict.TryGetValue(neighborsWithEdge.Vertex, out DijkstraVertexInfo neighborInfo))
                    {
                        var alt = curVertexInfo.Distance + neighborsWithEdge.Edge.Weight ?? 1;
                        if (alt < neighborInfo.Distance)
                        {
                            neighborInfo.Distance = alt;
                            neighborInfo.Parent = curVertexInfo;
                        }
                    }
                }
            }

            if (endInfo.Parent != null)
            {
                var reslut = new List<TVertex>();
                var curVertex = endInfo;
                while (curVertex != null)
                {
                    reslut.Add(curVertex.Vertex);
                    curVertex = curVertex.Parent;
                }
                reslut.Reverse();
                return reslut;
            }

            return null;
        }
    }
}

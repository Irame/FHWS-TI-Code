using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphs.Utils;
using QuickGraph;

namespace Graphs
{
    partial class Graph<TVertex>
    {
        // complexity of O(Ef)
        // E = number of edges (BFS part)
        // f = maximum flow (main loop)
        public double GetGreatestFlowWithFordFulkerson(TVertex source, TVertex sink)
        {
            var workingGraph = UglyPreperation();

            double maxFlow = 0;

            while (CheckForPathWithBfs(workingGraph, source, sink, out Dictionary<TVertex, EdgeBase<TVertex>> edgesToParent))
            {
                double pathFlow = double.PositiveInfinity;
                var s = sink;
                while (s != source)
                {
                    var edge = edgesToParent[s];
                    pathFlow = Math.Min(pathFlow, edge.Weight ?? 0);
                    s = s.GetOtherVertex(edge);
                }
                maxFlow += pathFlow;

                var v = sink;
                while (v != source)
                {
                    var edge = edgesToParent[v];
                    edge.Weight -= pathFlow;
                    var u = v.GetOtherVertex(edge);
                    workingGraph.GetEdgeOrCreate(v, u).Weight += pathFlow;
                    v = u;
                }
            }

            return maxFlow;

            // terminates if it finds the target
            bool CheckForPathWithBfs(Graph<TVertex> graph, TVertex start, TVertex end, out Dictionary<TVertex, EdgeBase<TVertex>> edgesToParent)
            {
                edgesToParent = new Dictionary<TVertex, EdgeBase<TVertex>>();
                Queue<(TVertex Vertex, EdgeBase<TVertex> Edge)> queue = new Queue<(TVertex, EdgeBase<TVertex>)>();
                HashSet<TVertex> visited = new HashSet<TVertex>();
                queue.Enqueue((start, null));
                while (!queue.IsEmpty())
                {
                    var curVertexAndEdge = queue.Dequeue();
                    if ((curVertexAndEdge.Edge?.Weight ?? 1) <= 0 || visited.Contains(curVertexAndEdge.Vertex))
                        continue;

                    visited.Add(curVertexAndEdge.Vertex);
                    edgesToParent[curVertexAndEdge.Vertex] = curVertexAndEdge.Edge;

                    if (curVertexAndEdge.Vertex == end)
                        return true;

                    queue.EnqueueRange(graph
                        .GetNeighborsWithEdges(curVertexAndEdge.Vertex)
                        .Distinct());
                }
                return false;
            }

            // this is done to make sure we have a graph with only one connection between every vertex and no self loops
            Graph<TVertex> UglyPreperation()
            {
                var edges = Edges
                    .GroupBy(edge => edge.Source)
                    .SelectMany(sourceGrouping => sourceGrouping
                        .GroupBy(edge => edge.Target)
                        .Select(targetGrouping => new EdgeBase<TVertex>(sourceGrouping.Key, targetGrouping.Key, targetGrouping.Sum(edge => edge.Weight))))
                    .GroupBy(edge =>
                        CultureInfo.CurrentCulture.CompareInfo.Compare(edge.Source.Name, edge.Target.Name) < 0
                            ? (edge.Target, edge.Source)
                            : (edge.Source, edge.Target))
                    .Select(grouping => {
                        var grp = grouping.ToArray();
                        if (grp.Length == 1)
                            return grp[0];
                        else
                        {
                            var wDiff = grp[0].Weight - grp[1].Weight;
                            return wDiff >= 0
                                ? new EdgeBase<TVertex>(grp[0].Source, grp[0].Target, wDiff)
                                : new EdgeBase<TVertex>(grp[1].Source, grp[1].Target, -wDiff);
                        }
                    })
                    .Where(edge => edge.Source != edge.Target);

                var result = new Graph<TVertex>(true);
                result.AddVertexRange(Vertices);
                result.AddEdgeRange(edges);
                return result;
            }
        }
    }
}

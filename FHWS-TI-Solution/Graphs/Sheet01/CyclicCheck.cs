using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Graphs.Utils;

namespace Graphs
{
    partial class Graph<TVertex>
    {
        public bool IsCyclic()
        {
            if (IsDirected)
            {
                return FindStronglyConnectedComponents().Any(scc => scc.Count > 1);
            }
            else
            {
                // find cycles via DFS if a vertex can be reached from two different paths there is a circle
                var stack = new Stack<(TVertex Vertex, TVertex Parent)>();
                var visited = new HashSet<TVertex>();
                var startingVertex = Vertices.First();
                while (startingVertex != null)
                {
                    stack.Push((startingVertex, null));
                    while (!stack.IsEmpty())
                    {
                        var curVertexParentPair = stack.Pop();
                        if (!visited.Contains(curVertexParentPair.Vertex))
                        {
                            visited.Add(curVertexParentPair.Vertex);
                            var neighbors = GetNeighbors(curVertexParentPair.Vertex, ignoreSelfLoops: true).Distinct()
                                .Where(vertex => vertex != curVertexParentPair.Parent).ToList();

                            if (neighbors.Any(vertex => visited.Contains(vertex)))
                                return true;

                            stack.PushRange(neighbors.Select(vertex => (vertex, curVertexParentPair.Vertex)));
                        }
                        else
                            return true;
                    }
                    startingVertex = Vertices.Except(visited).First();
                }
                return false;
            }
        }

        private class TarjanVertexInfo
        {
            public TVertex Vertex { get; }
            public int? Index { get; set; }
            public int LowLink { get; set; }
            public bool OnStack { get; set; }

            public TarjanVertexInfo(TVertex vertex)
            {
                Vertex = vertex;
                Index = null;
                LowLink = 0;
                OnStack = false;
            }
        }

        // finds StronglyConnectedComponents via the Tarjan Algorithm
        public List<List<TVertex>> FindStronglyConnectedComponents()
        {
            var stack = new Stack<TarjanVertexInfo>();
            List<List<TVertex>> scc = new List<List<TVertex>>();
            var vertexInfos = Vertices.ToDictionary(vertex => vertex, vertex => new TarjanVertexInfo(vertex));
            foreach (var vertexInfo in vertexInfos.Values)
            {
                if (vertexInfo.Index == null)
                    StrongConnect(vertexInfo);
            }
            return scc;

            void StrongConnect(TarjanVertexInfo vertexInfo, int index = 0)
            {
                vertexInfo.Index = index;
                vertexInfo.LowLink = index;
                vertexInfo.OnStack = true;
                stack.Push(vertexInfo);
                index++;

                foreach (var neighbor in GetNeighbors(vertexInfo.Vertex, ignoreSelfLoops: true))
                {
                    var neighborInfo = vertexInfos[neighbor];
                    if (neighborInfo.Index == null)
                    {
                        StrongConnect(neighborInfo, index++);
                        vertexInfo.LowLink = Math.Min(vertexInfo.LowLink, neighborInfo.LowLink);
                    }
                    else if (neighborInfo.OnStack)
                    {
                        vertexInfo.LowLink = Math.Min(vertexInfo.LowLink, neighborInfo.LowLink);
                    }
                }

                if (vertexInfo.LowLink == vertexInfo.Index.Value)
                {
                    TarjanVertexInfo connectedVertexInfo;
                    List<TVertex> curScc = new List<TVertex>();
                    do
                    {
                        (connectedVertexInfo = stack.Pop()).OnStack = false;
                        curScc.Add(connectedVertexInfo.Vertex);
                    }
                    while (connectedVertexInfo != vertexInfo);
                    scc.Add(curScc);
                }
            }
        }
    }

}

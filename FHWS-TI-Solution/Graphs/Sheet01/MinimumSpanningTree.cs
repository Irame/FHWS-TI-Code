using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphs.Utils;

namespace Graphs
{
    partial class Graph<TVertex>
    {
        public Graph<TVertex> GetMinimalSpanningTreeWithKruskal()
        {
            if (IsDirected)
            {
                // Edmonds' algorithm
                throw new NotImplementedException("GetMinimalSpanningTreeWithKruskal is not implemented for directed graphs.");
            }
            else
            {
                var result = new Graph<TVertex>();
                foreach (var edge in Edges.OrderBy(edge => edge.Weight))
                {
                    result.AddEdge(edge);
                    if (result.IsCyclic())
                    {
                        result.RemoveEdge(edge);
                        if (result.GetDegree(edge.Source) == 0)
                            result.RemoveVertex(edge.Source);
                        if (result.GetDegree(edge.Target) == 0)
                            result.RemoveVertex(edge.Target);
                    }
                }
                return result;
            }
        }
    }
}

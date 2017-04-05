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
                    var temp = new Graph<TVertex>(result);
                    temp.AddEdge(edge);
                    if (!temp.IsCyclic())
                    {
                        result = temp;
                    }
                }
                return result;
            }
        }
    }
}

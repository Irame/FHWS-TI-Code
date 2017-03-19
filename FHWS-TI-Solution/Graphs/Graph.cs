using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Common.Interfaces;
using GraphX.PCL.Common.Models;
using QuickGraph;
using QuickGraph.Collections;

namespace Graphs
{
    class UndirectedGraph<TData> : UndirectedGraph<Vertex<TData>, IEdge<Vertex<TData>>> {}

    class DirectedGraph<TData> : BidirectionalGraph<Vertex<TData>, IEdge<Vertex<TData>>> {}

    class Vertex<T>
    {
        public string Name { get; set; }

        public T Data { get; set; }

        public override string ToString()
        {
            return $"Vertex{{Name={Name}, Data={Data}}}";
        }
    }
}

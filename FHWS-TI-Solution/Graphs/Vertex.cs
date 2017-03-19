using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphs
{
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

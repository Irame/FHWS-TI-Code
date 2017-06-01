using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphs.Utils;
using MoreLinq;

namespace Graphs
{
    partial class Graph<TVertex>
    {
        public List<TVertex> NearestInsertion()
        {
            List<TVertex> C = new List<TVertex>();
            List<TVertex> freeVertecies = new List<TVertex>(Vertices);
            int i = 0;
            while (!freeVertecies.IsEmpty())
            {
                var vertex = SelectCandidate(freeVertecies, C);
                freeVertecies.Remove(vertex);
                Insert(vertex, C);
            }

            return C;

            TVertex SelectCandidate(List<TVertex> fv, List<TVertex> uv)
            {
                if (uv.Count == 0)
                    return fv.First();

                double minValue = double.MaxValue;
                TVertex candidate = null;
                foreach (var freeVertex in fv)
                {
                    foreach (var usedVertex in uv)
                    {
                        var weight = GetEdge(freeVertex, usedVertex).Weight ?? 1;
                        if (minValue > weight)
                        {
                            minValue = weight;
                            candidate = freeVertex;
                        }
                    }
                }
                return candidate;
            }
        }

        public List<TVertex> FarestInsertion()
        {
            List<TVertex> C = new List<TVertex>();
            List<TVertex> freeVertecies = new List<TVertex>(Vertices);
            int i = 0;
            while (!freeVertecies.IsEmpty())
            {
                var vertex = SelectCandidate(freeVertecies, C);
                freeVertecies.Remove(vertex);
                Insert(vertex, C);
            }

            return C;

            TVertex SelectCandidate(List<TVertex> fv, List<TVertex> uv)
            {
                if (uv.Count == 0)
                    return fv.First();

                double maxMinValue = 0;
                TVertex candidate = null;
                foreach (var freeVertex in fv)
                {
                    double minValue = double.MaxValue;
                    foreach (var usedVertex in uv)
                    {
                        var weight = GetEdge(freeVertex, usedVertex).Weight ?? 1;
                        if (minValue > weight) minValue = weight;
                    }

                    if (maxMinValue < minValue)
                    {
                        maxMinValue = minValue;
                        candidate = freeVertex;
                    }
                }
                return candidate;
            }
        }

        public List<TVertex> RandomInsertion()
        {
            List<TVertex> C = new List<TVertex>();
            int i = 0;
            foreach (var vertex in Vertices.Shuffle())
            {
                if (i < 3)
                {
                    C.Add(vertex);
                }
                else
                {
                    double minValue = double.MaxValue;
                    int minIndex = -1;
                    for (int j = 0; j < C.Count; j++)
                    {
                        int jp1 = (j + 1) % C.Count;
                        double newValue = GetEdge(C[j], vertex).Weight ??
                                          1 + GetEdge(vertex, C[jp1]).Weight ?? 1 - GetEdge(C[j], C[jp1]).Weight ?? 1;
                        if (minValue > newValue)
                            minIndex = j;
                    }
                    if (minIndex >= 0)
                        C.Insert(minIndex + 1, vertex);
                }

                i++;
            }

            return C;
        }

        private void Insert(TVertex vertex, List<TVertex> c)
        {
            if (c.Count < 3)
            {
                c.Add(vertex);
            }
            else
            {
                double minValue = double.MaxValue;
                int minIndex = -1;
                for (int j = 0; j < c.Count; j++)
                {
                    int jp1 = (j + 1) % c.Count;
                    double newValue = GetEdge(c[j], vertex).Weight ??
                                      1 + GetEdge(vertex, c[jp1]).Weight ?? 1 - GetEdge(c[j], c[jp1]).Weight ?? 1;
                    if (minValue > newValue)
                        minIndex = j;
                }
                if (minIndex >= 0)
                    c.Insert(minIndex + 1, vertex);
            }
        }
    }
}

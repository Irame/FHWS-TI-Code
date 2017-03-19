using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GraphX.PCL.Common.Models;
using GraphX.PCL.Logic.Algorithms;
using QuickGraph;

namespace Graphs
{
    static class FileParser
    {
        private static readonly Regex ParseVertexLineRegex = new Regex(@"knoten (?<name>[^\s]+)( (?<data>.+))?", RegexOptions.Compiled);
        private static readonly Regex ParseEdgeLineRegex = new Regex(@"kante (?<from>[^\s]+) (?<to>[^\s]+)( (?<weight>\d+))?", RegexOptions.Compiled);

        private static TGraph ParseFileToGraph<TGraph, TData, TEdge>(
            string filePath, 
            Func<string, TData> dataParser, 
            Func<string, Dictionary<string, Vertex<TData>>, TEdge> edgeParser)
            where TEdge: IEdge<Vertex<TData>>
            where TGraph: IMutableEdgeListGraph<Vertex<TData>, TEdge>, IMutableVertexSet<Vertex<TData>>, new()
        {
            var vertexDict = new Dictionary<string, Vertex<TData>>();
            var result = new TGraph();

            using (var file = new StreamReader(filePath))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    if (String.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                        continue;
                    line = line.Trim();
                    var vertexMatch = ParseVertexLineRegex.Match(line);
                    if (vertexMatch.Success)
                    {
                        var newVertex = new Vertex<TData>
                        {
                            Name = vertexMatch.Groups["name"].Value,
                            Data = vertexMatch.Groups["data"].Success ? dataParser(vertexMatch.Groups["data"].Value) : default(TData)
                        };
                        vertexDict.Add(newVertex.Name, newVertex);
                        result.AddVertex(newVertex);
                        continue;
                    }
                    var newEdge = edgeParser(line, vertexDict);
                    if (newEdge != null)
                    {
                        result.AddEdge(newEdge);
                    }
                }
            }
            return result;
        }

        public static UndirectedGraph<TData> ParseFileToUndirectedGraph<TData>(string filePath, Func<string, TData> dataParser)
        {
            return ParseFileToGraph<UndirectedGraph<TData>, TData, IEdge<Vertex<TData>>> (filePath, dataParser, 
                (edgeLine, vertexDict) =>
                {
                    var edgeMatch = ParseEdgeLineRegex.Match(edgeLine);
                    if (edgeMatch.Success)
                    {
                        UndirectedEdge<Vertex<TData>> newEdge;

                        if (edgeMatch.Groups["weight"].Success && double.TryParse(edgeMatch.Groups["weight"].Value, out double weight))
                        {
                            newEdge = new TaggedUndirectedEdge<Vertex<TData>, double>(
                                vertexDict[edgeMatch.Groups["from"].Value],
                                vertexDict[edgeMatch.Groups["to"].Value],
                                weight
                            );
                        }
                        else
                        {
                            newEdge = new UndirectedEdge<Vertex<TData>>(
                                vertexDict[edgeMatch.Groups["from"].Value],
                                vertexDict[edgeMatch.Groups["to"].Value]
                            );
                        }

                        return newEdge;
                    }
                    return null;
                });
        }

        public static UndirectedGraph<string> ParseFileToUndirectedGraph(string filePath)
        {
            return ParseFileToUndirectedGraph(filePath, dataString => dataString);
        }


        public static DirectedGraph<TData> ParseFileToBidirectionalGraph<TData>(string filePath, Func<string, TData> dataParser)
        {
            return ParseFileToGraph<DirectedGraph<TData>, TData, IEdge<Vertex<TData>>> (filePath, dataParser, 
                (edgeLine, vertexDict) =>
                {
                    var edgeMatch = ParseEdgeLineRegex.Match(edgeLine);
                    if (edgeMatch.Success)
                    {
                        Edge<Vertex<TData>> newEdge;

                        if (edgeMatch.Groups["weight"].Success && double.TryParse(edgeMatch.Groups["weight"].Value, out double weight))
                        {
                            newEdge = new TaggedEdge<Vertex<TData>, double>(
                                vertexDict[edgeMatch.Groups["from"].Value],
                                vertexDict[edgeMatch.Groups["to"].Value],
                                weight
                            );
                        }
                        else
                        {
                            newEdge = new Edge<Vertex<TData>>(
                                vertexDict[edgeMatch.Groups["from"].Value],
                                vertexDict[edgeMatch.Groups["to"].Value]
                            );
                        }

                        return newEdge;
                    }
                    return null;
                });
        }


        public static DirectedGraph<string> ParseFileToBidirectionalGraph(string filePath)
        {
            return ParseFileToBidirectionalGraph(filePath, dataString => dataString);
        }
    }
}

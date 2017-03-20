using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Graphs
{
    static class FileParser
    {
        private static readonly Regex ParseVertexLineRegex = new Regex(@"knoten (?<name>[^\s]+)( (?<data>.+))?", RegexOptions.Compiled);
        private static readonly Regex ParseEdgeLineRegex = new Regex(@"kante (?<from>[^\s]+) (?<to>[^\s]+)( (?<weight>\d+))?", RegexOptions.Compiled);

        public static Graph<VertexBase>  ParseFileToGraph(string filePath, bool isDirected = false)
        {
            var vertexDict = new Dictionary<string, VertexBase>();
            var result = new Graph<VertexBase>(isDirected);

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
                        var newVertex = new VertexBase
                        {
                            Name = vertexMatch.Groups["name"].Value,
                            Data = vertexMatch.Groups["data"].Success ? vertexMatch.Groups["data"].Value : null
                        };
                        vertexDict.Add(newVertex.Name, newVertex);
                        result.AddVertex(newVertex);
                        continue;
                    }

                    var edgeMatch = ParseEdgeLineRegex.Match(line);
                    if (edgeMatch.Success)
                    {
                        var newEdge = new EdgeBase<VertexBase>(
                            vertexDict[edgeMatch.Groups["from"].Value],
                            vertexDict[edgeMatch.Groups["to"].Value]);

                        if (edgeMatch.Groups["weight"].Success && double.TryParse(edgeMatch.Groups["weight"].Value, out double weight))
                            newEdge.Weight = weight;

                        result.AddEdge(newEdge);
                    }
                }
            }
            return result;
        }
    }
}

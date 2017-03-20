using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using GraphX;
using GraphX.Controls;
using GraphX.Controls.Models;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Logic.Models;
using QuickGraph;

namespace Graphs
{
    class VisualGraphArea : GraphArea<VisualVertex, VisualEdge, BidirectionalGraph<VisualVertex, VisualEdge>>
    {
        public VisualGraphArea()
        {
            LogicCore = new GXLogicCore
                <VisualVertex, VisualEdge, BidirectionalGraph<VisualVertex, VisualEdge>>
                {
                    DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.KK
                };
            ControlFactory = new VisualGraphControlFactory(this);
            EdgeLabelFactory = new DefaultEdgelabelFactory();
        }

        public void UpdateGraph(Graph<VertexBase> newGraph)
        {
            var graph = new BidirectionalGraph<VisualVertex, VisualEdge>();
            var vertexDict = newGraph.NameVertexDictionary.ToDictionary(kvp => kvp.Key, kvp => new VisualVertex(kvp.Value));

            graph.AddVertexRange(vertexDict.Values);
            graph.AddEdgeRange(newGraph.EdgeList.Select(edge => new VisualEdge(edge, vertexDict)));

            ShowAllEdgesArrows(newGraph.IsDirected);
            ClearLayout();
            GenerateGraph(graph);
        }

        class VisualGraphControlFactory : GraphControlFactory
        {
            public VisualGraphControlFactory(GraphAreaBase graphArea)
                : base(graphArea)
            {
            }

            public override EdgeControl CreateEdgeControl(VertexControl source, VertexControl target, object edge, bool showLabels = false, bool showArrows = true, Visibility visibility = Visibility.Visible)
            {
                var result = new VisualEdgeControl()
                {
                    Source = source,
                    Target = target,
                    Edge = edge,
                    ShowLabel = showLabels,
                    ShowArrows = showArrows,
                    Visibility = visibility
                };

                return result;
            }
        }

        class VisualEdgeControl : EdgeControl
        {
            public override void OnApplyTemplate()
            {
                base.OnApplyTemplate();

                if (!ShowArrows)
                {
                    // Hide arrow
                    EdgePointerForSource?.Hide();
                    EdgePointerForTarget?.Hide();

                    // Force line length to connect source/target
                    Geometry linegeometry = new PathGeometry(new[]
                    {
                        new PathFigure(Source.GetCenterPosition(), new [] {
                            new LineSegment(Target.GetCenterPosition(), isStroked: true)
                        }, closed: false)
                    });

                    LinePathObject.Data = linegeometry;
                }
            }
        }
    }

    class VisualVertex : GraphX.PCL.Common.Models.VertexBase
    {
        public VertexBase Vertex { get; }

        public VisualVertex(VertexBase vertex)
        {
            Vertex = vertex;
        }

        public override string ToString()
        {
            return $"{Vertex.Name}: {Vertex.Data}";
        }
    }

    class VisualEdge : GraphX.PCL.Common.Models.EdgeBase<VisualVertex>
    {
        public EdgeBase<VertexBase> Edge { get; }

        public VisualEdge(EdgeBase<VertexBase> edge, Dictionary<string, VisualVertex> vertexDict) : base(vertexDict[edge.Source.Name], vertexDict[edge.Target.Name])
        {
            Edge = edge;
            Weight = edge.Weight ?? Weight;
        }

        public override string ToString()
        {
            return $"{Edge.Weight}";
        }
    }
}

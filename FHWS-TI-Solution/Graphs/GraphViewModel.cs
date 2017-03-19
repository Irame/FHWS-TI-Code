using System;
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
    class GraphViewModel<TData> : GraphArea<GraphXVertex<TData>, GraphXEdge<TData>, BidirectionalGraph<GraphXVertex<TData>, GraphXEdge<TData>>>
    {
        public GraphViewModel()
        {
            LogicCore = new GXLogicCore
                <GraphXVertex<TData>, GraphXEdge<TData>, BidirectionalGraph<GraphXVertex<TData>, GraphXEdge<TData>>>
                {
                    DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.KK
                };
            ControlFactory = new VisualGraphControlFactory(this);
        }

        public void UpdateGraph(IEdgeListGraph<Vertex<TData>, IEdge<Vertex<TData>>> newGraph)
        {
            var graph = new BidirectionalGraph<GraphXVertex<TData>, GraphXEdge<TData>>();
            var vertexDict = new Dictionary<string, GraphXVertex<TData>>();

            foreach (var newGraphVertex in newGraph.Vertices)
            {
                var newVertex = new GraphXVertex<TData>(newGraphVertex);
                vertexDict[newVertex.Name] = newVertex;
                graph.AddVertex(newVertex);
            }

            foreach (var newGraphEdge in newGraph.Edges)
            {
                graph.AddEdge(new GraphXEdge<TData>(newGraphEdge, vertexDict));
            }

            ShowAllEdgesArrows(newGraph is DirectedGraph<TData>);
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
}

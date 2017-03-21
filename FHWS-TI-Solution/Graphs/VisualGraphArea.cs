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
using GraphX.PCL.Logic.Algorithms.LayoutAlgorithms;
using GraphX.PCL.Logic.Models;
using QuickGraph;

namespace Graphs
{
    class VisualGraphArea : GraphArea<VisualVertex, VisualEdge, BidirectionalGraph<VisualVertex, VisualEdge>>
    {
        public VisualGraphArea()
        {
            LogicCore = new GXLogicCore<VisualVertex, VisualEdge, BidirectionalGraph<VisualVertex, VisualEdge>>
            {
                DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.BoundedFR,
                DefaultLayoutAlgorithmParams = new BoundedFRLayoutParameters
                {
                    Width = 400,
                    Height = 400
                },
                EnableParallelEdges = true,
                EdgeCurvingEnabled = true,
                DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.SimpleER

            };
            ControlFactory = new VisualGraphControlFactory(this);
            EdgeLabelFactory = new DefaultEdgelabelFactory();
            
            SetVerticesDrag(true, true);
            ShowAllEdgesLabels();
            //AlignAllEdgesLabels();
        }

        public void UpdateGraph(Graph<VertexBase> newGraph)
        {
            var graph = new BidirectionalGraph<VisualVertex, VisualEdge>();
            var vertexDict = newGraph.NameVertexDictionary.ToDictionary(kvp => kvp.Key, kvp => new VisualVertex(kvp.Value));

            graph.AddVertexRange(vertexDict.Values);
            graph.AddEdgeRange(newGraph.Edges.Select(edge => new VisualEdge(edge, vertexDict)));

            ShowAllEdgesArrows(newGraph.IsDirected);
            ClearLayout();
            GenerateGraph(graph);
        }

        public static readonly DependencyProperty GraphProperty = DependencyProperty.Register(
            "Graph", typeof(Graph<VertexBase>), typeof(VisualGraphArea), 
            new PropertyMetadata(default(Graph<VertexBase>), 
                (sender, args) => (sender as VisualGraphArea)?.UpdateGraph(args.NewValue as Graph<VertexBase>)));

        public Graph<VertexBase> Graph
        {
            get { return (Graph<VertexBase>) GetValue(GraphProperty); }
            set { SetValue(GraphProperty, value); }
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

                var origEdgeLabel = GetTemplatePart("PART_edgeLabel") as IEdgeLabelControl;
                origEdgeLabel?.Hide();
            }

            public override void UpdateEdge(bool updateLabel = true)
            {
                base.UpdateEdge(updateLabel);
                if (!ShowArrows && LinePathObject != null)
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

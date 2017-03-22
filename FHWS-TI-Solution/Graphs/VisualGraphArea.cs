using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
            AlignAllEdgesLabels();
        }

        private void UpdateGraph(Graph<VertexBase> newGraph)
        {
            var graph = new BidirectionalGraph<VisualVertex, VisualEdge>();
            var vertexDict = newGraph.NameVertexDictionary.ToDictionary(kvp => kvp.Key, kvp => new VisualVertex(kvp.Value));

            graph.AddVertexRange(vertexDict.Values);
            graph.AddEdgeRange(newGraph.Edges.Select(edge => new VisualEdge(edge, vertexDict)));

            ShowAllEdgesArrows(newGraph.IsDirected);
            ClearLayout();
            GenerateGraph(graph);

            if (Parent is ZoomControl zoomControl) zoomControl.ZoomToFill();
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
            {}

            public override VertexControl CreateVertexControl(object vertexData)
            {
                if (vertexData is VisualVertex vVertex)
                    return new VisualVertexControl(vVertex);
                else
                    return new VertexControl(vertexData);
            }

            public override EdgeControl CreateEdgeControl(VertexControl source, VertexControl target, object edge, bool showLabels = false, bool showArrows = true, Visibility visibility = Visibility.Visible)
            {
                return new VisualEdgeControl(source, target, (VisualEdge)edge, showLabels, showArrows) { Visibility = visibility };
            }
        }

        class VisualVertexControl : VertexControl
        {
            public VisualVertexControl(VisualVertex vertexData, bool tracePositionChange = true,
                bool bindToDataObject = true) : base(vertexData, tracePositionChange, bindToDataObject)
            {
                BindingOperations.SetBinding(this, ForegroundProperty, new Binding("ForegroundBrush")
                {
                    Source = vertexData.Vertex,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

                BindingOperations.SetBinding(this, BackgroundProperty, new Binding("BackgroundBrush")
                {
                    Source = vertexData.Vertex,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
            }
        }

        class VisualEdgeControl : EdgeControl
        {
            public VisualEdgeControl(VertexControl source, VertexControl target, VisualEdge edgeData, bool showLabels = false, bool showArrows = true) : base(source, target, edgeData, showLabels, showArrows)
            {
                BindingOperations.SetBinding(this, ForegroundProperty, new Binding("StrokeBrush")
                {
                    Source = edgeData.Edge,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
            }

            public override void OnApplyTemplate()
            {
                base.OnApplyTemplate();

                var origEdgeLabel = GetTemplatePart("PART_edgeLabel") as IEdgeLabelControl;
                origEdgeLabel?.Hide();
            }

            public override void UpdateEdge(bool updateLabel = true)
            {
                if (!ShowArrows)
                {
                    if (EdgePointerForTarget != null)
                    {
                        EdgePointerForTarget?.Hide();
                        EdgePointerForTarget = null;
                    }
                    if (EdgePointerForSource != null)
                    {
                        EdgePointerForSource?.Hide();
                        EdgePointerForSource = null;
                    }
                }
                base.UpdateEdge(updateLabel);
                if (((VisualEdge)Edge).Edge.Weight == null)
                    EdgeLabelControl?.Hide();
                else
                    EdgeLabelControl?.Show();
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

        public new double Weight => Edge.Weight ?? base.Weight;

        public VisualEdge(EdgeBase<VertexBase> edge, Dictionary<string, VisualVertex> vertexDict) : base(vertexDict[edge.Source.Name], vertexDict[edge.Target.Name])
        {
            Edge = edge;
        }

        public override string ToString()
        {
            return $"{Weight}";
        }
    }
}

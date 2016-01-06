using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MicroFlow.Graph
{
    using Bag = Dictionary<string, object>;

    public class FlowGraphBuilder : NodeVisitor
    {
        private static readonly XNamespace RootNamespace = "http://schemas.microsoft.com/vs/2009/dgml";

        private readonly XElement _nodes;
        private readonly XElement _links;

        public FlowGraphBuilder()
        {
            Result = new XElement(RootNamespace + "DirectedGraph");
            
            _nodes = new XElement(RootNamespace + "Nodes");
            _links = new XElement(RootNamespace + "Links");

            Result.Add(_nodes);
            Result.Add(_links);

            AddCategories();
        }

        public XElement Result { get; }

        public XElement GenerateDgml(FlowBuilder flowBuilder)
        {
            if (flowBuilder == null) throw new ArgumentNullException(nameof(flowBuilder));

            foreach (var node in flowBuilder.Nodes)
            {
                node.Accept(this);
            }

            return Result;
        }

        protected override void VisitActivity<TActivity>(ActivityNode<TActivity> activityNode)
        {
            AddNode(
                activityNode.Id,
                activityNode.Name ?? typeof(TActivity).Name,
                activityNode.ToCategory(),
                new Bag { ["ActivityType"] = typeof(TActivity).Name });

            AddLink(activityNode, activityNode.PointsTo, Categories.NormalFlowEdge);
            AddLink(activityNode, activityNode.FaultHandler, Categories.FaultFlowEdge);
            AddLink(activityNode, activityNode.CancellationHandler, Categories.CancellationFlowEdge);
        }

        protected override void VisitSwitch<TChoice>(SwitchNode<TChoice> switchNode)
        {
            AddNode(switchNode);

            AddLink(switchNode, switchNode.DefaultCase, "Default");

            foreach (var valueNodePair in switchNode.Cases)
            {
                AddLink(switchNode, valueNodePair.Value, Categories.NormalFlowEdge, valueNodePair.Key?.ToString() ?? "");
            }
        }

        protected override void VisitCondition(ConditionNode conditionNode)
        {
            AddNode(conditionNode);

            AddLink(conditionNode, conditionNode.WhenFalse, Categories.NormalFlowEdge, "False");
            AddLink(conditionNode, conditionNode.WhenTrue, Categories.NormalFlowEdge, "True");
        }

        protected override void VisitForkJoin(ForkJoinNode forkJoinNode)
        {
            AddNode(forkJoinNode);

            foreach (var fork in forkJoinNode.Forks)
            {
                AddNode(
                    fork.Id, fork.Name, Categories.ForkNode, 
                    new Bag {["ActivityType"] = fork.ActivityType.Name });

                AddLink(forkJoinNode.Id, fork.Id, Categories.ParallelFlowEdge);

                AddLink(fork.Id, forkJoinNode.PointsTo?.Id, Categories.NormalFlowEdge);
                AddLink(fork.Id, forkJoinNode.FaultHandler?.Id, Categories.FaultFlowEdge);
                AddLink(fork.Id, forkJoinNode.CancellationHandler?.Id, Categories.CancellationFlowEdge);
            }
        }

        protected override void VisitBlock(BlockNode blockNode)
        {
            AddNode(blockNode, new Bag { ["Group"] = "Expanded" });

            foreach (var innerNode in blockNode.InnerNodes)
            {
                AddLink(blockNode, innerNode, "Contains");
            }

            AddLink(blockNode, blockNode.PointsTo, Categories.NormalFlowEdge);
        }

        private void AddNode(IFlowNode node, Bag properties = null)
        {
            AddNode(node.Id, node.Name, node.ToCategory(), properties);
        }

        private void AddNode(Guid id, string name, string category = null, Dictionary<string, object> properties = null)
        {
            var node = new XElement(
                RootNamespace + "Node",
                new XAttribute("Id", id),
                new XAttribute("Label", name ?? ""), 
                properties.ToAttributes());

            node.SetOptionalAttribute("Category", category);

            _nodes.Add(node);
        }

        private void AddLink(IFlowNode from, IFlowNode to, string category = null, string label = null)
        {
            AddLink(from.Id, to?.Id, category, label);
        }

        private void AddLink(Guid? from, Guid? to, string category = null, string label = null)
        {
            if (from == null || to == null) return;

            var link = new XElement(
                RootNamespace + "Link", 
                new XAttribute("Source", from), 
                new XAttribute("Target", to));

            link.SetOptionalAttribute("Category", category);
            link.SetOptionalAttribute("Label", label);

            _links.Add(link);
        }

        private void AddCategories()
        {
            var nodeCategories = Categories
                .NodeCategories
                .Select(c => new XElement(
                    RootNamespace + "Category",
                    new XAttribute("Id", c),
                    new XAttribute("Background", Background.OfCategory(c))));

            var categories = new XElement(RootNamespace + "Categories", nodeCategories);

            Result.Add(categories);
        }
    }
}
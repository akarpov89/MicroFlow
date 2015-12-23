using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace MicroFlow
{
    public static class BlockAcyclityChecker
    {
        public static bool IsAcyclic([NotNull] BlockNode block)
        {
            block.AssertNotNull("block != null");
            if (block.InnerNodes.Count == 0) return true;

            var visited = new Stack<IFlowNode>();

            bool hasCycle = HasCycle(block.InnerNodes[0], visited);
            visited.Clear();

            return !hasCycle;
        }

        private static bool HasCycle(IFlowNode node, Stack<IFlowNode> visited)
        {
            visited.Push(node);
            int length = visited.Count;

            foreach (IFlowNode neighbor in node.GetNeighbors())
            {
                while (visited.Count > length)
                {
                    visited.Pop();
                }

                if (visited.Contains(neighbor)) return true;
                if (HasCycle(neighbor, visited)) return true;
            }

            return false;
        }
    }
}
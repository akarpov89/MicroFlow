using System.Collections.Generic;

namespace MicroFlow.Graph
{
    public static class Background
    {
        public static string OfCategory(string category)
        {
            string color;
            if (!Colors.TryGetValue(category, out color))
            {
                color = "#EEEEEE";
            }
            return color;
        }

        private static readonly Dictionary<string, string> Colors = new Dictionary<string, string>
        {
            [Categories.ActivityNode] = "DarkBlue",
            [Categories.ConditionNode] = "Goldenrod",
            [Categories.SwitchNode] = "Purple",
            [Categories.ForkJoinNode] = "#FF672878",
            [Categories.ForkNode] = "Green",
            [Categories.BlockNode] = "#EEEEEE",
            [Categories.FaultHandlerNode] = "Brown"
        };
    }
}
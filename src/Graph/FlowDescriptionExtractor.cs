using System;
using System.Reflection;
using JetBrains.Annotations;

namespace MicroFlow.Graph
{
    public static class FlowDescriptionExtractor
    {
        [NotNull]
        public static FlowDescription ExtractFrom([NotNull] Type type)
        {
            if (!typeof (Flow).IsAssignableFrom(type))
            {
                throw new InvalidOperationException("Type isn't a Flow");
            }

            var flow = (Flow) TypeUtils.CreateDefaultConstructorFactoryFor(type)();

            var buildMethod = type.GetMethod("Build", BindingFlags.Instance | BindingFlags.NonPublic);

            var flowBuilder = new FlowBuilder();

            buildMethod.Invoke(flow, new object[] {flowBuilder});

            return flowBuilder.CreateFlow();
        }
    }
}
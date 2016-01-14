using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MicroFlow.Graph
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: MicroFlow.Graph path_to_assembly flow_class_name");
                return;
            }

            string path = args[0];
            string className = args[1];
            string output = Path.Combine(Path.GetDirectoryName(path).NotNull(), className + ".dgml");

            try
            {
                Console.WriteLine("Load assembly...");
                var assembly = Assembly.LoadFrom(path);

                Console.WriteLine("Search type...");
                var flowType = assembly.GetTypes().FirstOrDefault(t => t.Name == className);

                if (flowType == null)
                {
                    Console.WriteLine("Type not found");
                    return;
                }

                Console.WriteLine($"Type {flowType.FullName} found");

                Console.WriteLine("Extract flow description...");
                var flowDescription = FlowDescriptionExtractor.ExtractFrom(flowType);

                Console.WriteLine("Generate graph...");
                var graph = new FlowGraphBuilder().GenerateDgml(flowDescription);

                Console.WriteLine($"Save to {output}");
                graph.Save(output);

                Console.WriteLine("Done!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}

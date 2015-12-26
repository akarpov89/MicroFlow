using System;
using MicroFlow;

namespace Sample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            RunSecondFlow();
        }

        private static void RunThirdFlow()
        {
            var flow = new MyFlowWithBlock();
            try
            {
                flow.Run().Wait();
            }
            catch (FlowValidationException e)
            {
                foreach (ValidationError err in e.ValidatonResult.Errors)
                {
                    Console.WriteLine("Node: {0}. Message: {1}", err.NodeName, err.Message);
                }
            }
        }

        private static void CheckAcyclity()
        {
            var b = new FlowBuilder();
            BlockNode block = b.Block("1", (thisBlock, builder) =>
            {
                ActivityNode<InputActivity> a1 = builder.Activity<InputActivity>();

                ConditionNode c = builder.Condition();
                ActivityNode<InputActivity> a2 = builder.Activity<InputActivity>();
                ActivityNode<InputActivity> a3 = builder.Activity<InputActivity>();

                ActivityNode<InputActivity> a4 = builder.Activity<InputActivity>();

                a1.ConnectTo(c);
                c.ConnectFalseTo(a2).ConnectTrueTo(a3);

                a2.ConnectTo(a4);
                a3.ConnectTo(a4);

                a4.ConnectTo(a1);
            });

            bool isAcyclic = BlockAcyclityChecker.IsAcyclic(block);
        }

        private static void RunFirstFlow()
        {
            var flow = new MyFlow();

            try
            {
                flow.Run();
            }
            catch (FlowValidationException e)
            {
                foreach (ValidationError error in e.ValidatonResult.Errors)
                {
                    Console.WriteLine($"{error.NodeName}: {error.Message}");
                }
            }
        }

        private static void RunSecondFlow()
        {
            var flow2 = new MySecondFlow();

            try
            {
                flow2.Run().Wait();
            }
            catch (FlowValidationException e)
            {
                foreach (ValidationError error in e.ValidatonResult.Errors)
                {
                    Console.WriteLine($"{error.NodeName}: {error.Message}");
                }
            }
        }
    }
}
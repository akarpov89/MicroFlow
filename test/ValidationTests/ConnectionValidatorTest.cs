using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace MicroFlow.Test
{
    [TestFixture]
    public class ConnectionValidatorTest
    {
        [Test, TestCaseSource(nameof(Cases))]
        public void RunCase(Func<FlowBuilder, IFlowNode> createNode, int errorsCount)
        {
            // Arrange
            var b = new FlowBuilder();

            var f = b.FaultHandler<MyFaultHandler>();
            var c = b.Activity<MyCancellationHandler>();
            b.WithDefaultFaultHandler(f);
            b.WithDefaultCancellationHandler(c);

            var node = createNode(b);
            b.WithInitialNode(node);

            var validator = new ConnectionValidator();

            // Act
            validator.Validate(b);

            // Assert
            Assert.That(validator.Result.GetErrorsOf(node).Count(), Is.EqualTo(errorsCount));
        }

        public static IEnumerable<TestCaseData> Cases
        {
            get
            {
                yield return Case(
                    "Activity", 
                    b => b.DummyActivity(), 
                    a => a.ConnectTo(a).ConnectCancellationTo(a), 
                    2);

                yield return Case(
                    "Condition",
                    b => b.Condition().WithCondition(() => true),
                    c => c.ConnectFalseTo(c).ConnectTrueTo(c),
                    2);

                yield return Case(
                    "Condition without condition expr",
                    b => b.Condition(),
                    c => { },
                    2);

                yield return Case(
                    "Switch",
                    b => b.SwitchOf<int>().WithChoice(() => 42),
                    s => s.ConnectCase(0).To(s).ConnectDefaultTo(s),
                    2);

                yield return Case(
                    "Switch without choice",
                    b => b.SwitchOf<int>(),
                    s => { },
                    2);

                yield return Case(
                    "Switch with replication",
                    b => b.SwitchOf<int>().WithChoice(() => 42),
                    s => s.ConnectCase(0).To(s).ConnectCase(0).To(s),
                    3);

                yield return Case(
                    "ForkJoin",
                    b => b.ForkJoin(),
                    f => f.ConnectTo(f).ConnectCancellationTo(f),
                    3);

                yield return Case(
                    "Block",
                    b => b.Block(),
                    b => b.ConnectTo(b),
                    1);
            }
        }

        private static TestCaseData Case<T>(
            string name,
            Func<FlowBuilder, T> create, Action<T> connect,
            int errorsCount)
            where T : IFlowNode
        {
            return new TestCaseData(
                new Func<FlowBuilder, IFlowNode>(b =>
                {
                    var node = create(b);
                    connect(node);
                    return node;
                }), 
                errorsCount)
                .SetName(name);
        }
    }
}
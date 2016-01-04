using System;
using NUnit.Framework;

namespace MicroFlow.Test
{
    using static BlockAcyclityChecker;

    [TestFixture]
    public class BlockAcyclityCheckerTest
    {
        [Test]
        public void Case1()
        {
            var block = BuildBlock(b =>
            {
                var a1 = b.DummyActivity();

                var c = b.Condition();
                var a2 = b.DummyActivity();
                var a3 = b.DummyActivity();
                var a4 = b.DummyActivity();

                a1.ConnectTo(c);
                c.ConnectFalseTo(a2).ConnectTrueTo(a3);

                a2.ConnectTo(a4);
                a3.ConnectTo(a4);

                a4.ConnectTo(a1);
            });

            Assert.That(IsAcyclic(block), Is.False);
        }

        [Test]
        public void Case2()
        {
            var block = BuildBlock(b =>
            {
                var a1 = b.DummyActivity();

                var c = b.Condition();
                var a2 = b.DummyActivity();
                var a3 = b.DummyActivity();
                var a4 = b.DummyActivity();

                a1.ConnectTo(c);
                c.ConnectFalseTo(a2).ConnectTrueTo(a3);

                a2.ConnectTo(a4);
                a3.ConnectTo(a4);
            });

            Assert.That(IsAcyclic(block), Is.True);
        }

        private static BlockNode BuildBlock(Action<FlowBuilder> buildAction)
        {
            var b = new FlowBuilder();
            return b.Block("1", (thisBlock, builder) => buildAction(builder));
        }
    }

    internal static class DummyExtensions
    {
        public static ActivityNode<DummyActivity> DummyActivity(this FlowBuilder builder)
        {
            return builder.Activity<DummyActivity>();
        }
    }
}
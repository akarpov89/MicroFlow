using NUnit.Framework;

namespace MicroFlow.Test
{
    [TestFixture]
    public class ReachabilityValidatorTest
    {
        [Test]
        public void CheckFlowWithActivity()
        {
            // Arrange
            var b = new FlowBuilder();
            var a1 = b.DummyActivity();
            var a2 = b.DummyActivity();

            b.WithInitialNode(a1);

            var validator = new ReachabilityValidator();

            // Act
            validator.Validate(b);
            var result = validator.Result;

            // Assert
            Assert.That(result.GetErrorsOf(a1), Is.Empty);
            Assert.That(result.GetErrorsOf(a2), Is.Not.Empty);
        }

        [Test]
        public void CheckFlowWithCondition()
        {
            // Arrange
            var b = new FlowBuilder();

            var c = b.Condition();
            var a = b.DummyActivity();

            b.WithInitialNode(c);
            c.ConnectFalseTo(a).ConnectTrueTo(a);

            var e = b.DummyActivity();

            var validator = new ReachabilityValidator();

            // Act
            validator.Validate(b);
            var result = validator.Result;

            // Assert
            Assert.That(result.GetErrorsOf(c), Is.Empty);
            Assert.That(result.GetErrorsOf(a), Is.Empty);
            Assert.That(result.GetErrorsOf(e), Is.Not.Empty);
        }

        [Test]
        public void CheckFlowWithSwitch()
        {
            // Arrange
            var b = new FlowBuilder();

            var s = b.SwitchOf<int>();
            var a1 = b.DummyActivity();
            var a2 = b.DummyActivity();
            var a3 = b.DummyActivity();

            b.WithInitialNode(s);
            s.ConnectCase(0).To(a1);
            s.ConnectCase(1).To(a2);
            s.ConnectDefaultTo(a3);

            var e = b.DummyActivity();

            var validator = new ReachabilityValidator();

            // Act
            validator.Validate(b);
            var result = validator.Result;

            // Assert
            Assert.That(result.GetErrorsOf(s), Is.Empty);
            Assert.That(result.GetErrorsOf(a1), Is.Empty);
            Assert.That(result.GetErrorsOf(a2), Is.Empty);
            Assert.That(result.GetErrorsOf(a3), Is.Empty);
            Assert.That(result.GetErrorsOf(e), Is.Not.Empty);
        }

        [Test]
        public void CheckFlowWithForkJoin()
        {
            // Arrange
            var b = new FlowBuilder();

            var f = b.ForkJoin();
            var e = b.DummyActivity();

            b.WithInitialNode(f);

            var validator = new ReachabilityValidator();

            // Act
            validator.Validate(b);
            var result = validator.Result;

            // Assert
            Assert.That(result.GetErrorsOf(f), Is.Empty);
            Assert.That(result.GetErrorsOf(e), Is.Not.Empty);
        }

        [Test]
        public void CheckFlowWithBlock()
        {
            // Arrange
            var b = new FlowBuilder();

            IFlowNode a = null;
            IFlowNode e = null;

            var block = b.Block("test", (_, builder) =>
            {
                a = builder.DummyActivity();
                e = builder.DummyActivity();
            });

            b.WithInitialNode(block);

            var validator = new ReachabilityValidator();

            // Act
            validator.Validate(b);
            var result = validator.Result;

            // Assert
            Assert.That(result.GetErrorsOf(block), Is.Empty);
            Assert.That(result.GetErrorsOf(a), Is.Empty);
            Assert.That(result.GetErrorsOf(e), Is.Not.Empty);
        }
    }
}
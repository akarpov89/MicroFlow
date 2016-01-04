using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;

namespace MicroFlow.Test
{
    [TestFixture]
    public class Flow3Tests
    {
        [Test]
        public void FlowIsValid()
        {
            // Arrange
            var reader = Substitute.For<IReader>();
            var writer = Substitute.For<IWriter>();
            var flow = new Flow3(reader, writer);

            // Act
            var validationResult = flow.Validate();

            // Assert
            Assert.That(validationResult.HasErrors, Is.False);
        }

        [Test, TestCaseSource(nameof(Cases))]
        public void RunCase(string inputNumber, string echoMessage)
        {
            // Arrange
            var reader = new ArrayReader(inputNumber);
            var writer = Substitute.For<IWriter>();
            var flow = new Flow3(reader, writer);

            // Act
            flow.Run().Wait();

            // Assert
            writer.Received().Write(echoMessage);
        }

        public static IEnumerable<TestCaseData> Cases
        {
            get
            {
                yield return new TestCaseData("1", "Echo: 1");
                yield return new TestCaseData("42", "Echo: 42");
            }
        }
    }
}
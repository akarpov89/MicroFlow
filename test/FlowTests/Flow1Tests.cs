using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;

namespace MicroFlow.Test
{
    [TestFixture]
    public class Flow1Tests
    {
        [Test]
        public void FlowIsValid()
        {
            // Arrange
            var reader = Substitute.For<IReader>();
            var writer = Substitute.For<IWriter>();
            var flow = new Flow1 {Reader = reader, Writer = writer};

            // Act
            var validationResult = flow.Validate();

            // Assert
            Assert.That(validationResult.HasErrors, Is.False);
        }

        [Test, TestCaseSource(nameof(Cases))]
        public void RunCase(string first, string second, string expectedMessage)
        {
            // Arrange
            var reader = new ArrayReader(first, second);
            var writer = Substitute.For<IWriter>();
            var flow = new Flow1 {Reader = reader, Writer = writer};

            // Act
            flow.Run().Wait();

            // Assert
            writer.Received().Write(expectedMessage);
        }

        public static IEnumerable<TestCaseData> Cases
        {
            get
            {
                yield return Case("1", "2", "1 <= 2");
                yield return Case("2", "1", "2 > 1");
                yield return Case("42", "42", "42 <= 42");
            }
        }

        public static TestCaseData Case(string first, string second, string message) 
            => new TestCaseData(first, second, message);
    }
}
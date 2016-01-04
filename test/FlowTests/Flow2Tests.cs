using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;

namespace MicroFlow.Test
{
    [TestFixture]
    public class Flow2Tests
    {
        [Test]
        public void FlowIsValid()
        {
            // Arrange
            var writer = Substitute.For<IWriter>();
            var flow = new Flow2(writer);

            // Act
            var validationResult = flow.Validate();

            // Assert
            Assert.That(validationResult.HasErrors, Is.False);
        }

        [Test, TestCaseSource(nameof(Cases))]
        public void RunCase(int a, int b, int c, string expectedMessage)
        {
            // Arrange
            var writer = Substitute.For<IWriter>();
            var flow = new Flow2(writer) {A = a, B = b, C = c};

            // Act
            flow.Run().Wait();

            // Assert
            writer.Received().Write(expectedMessage);
        }

        public static IEnumerable<TestCaseData> Cases
        {
            get
            {
                yield return Case(1, 2, 3, "2 + 3 + 4 = 9");
                yield return Case(0, 8, 19, "1 + 9 + 20 = 30");
            }
        }

        private static TestCaseData Case(int a, int b, int c, string message) => new TestCaseData(a, b, c, message);
    }
}
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;

namespace MicroFlow.Test
{
  [TestFixture]
  public class Flow4Tests
  {
    [Test]
    public void FlowIsValid()
    {
      // Arrange
      var writer = Substitute.For<IWriter>();
      var flow = new Flow4 {Writer = writer};

      // Act
      var validationResult = flow.Validate();

      // Assert
      Assert.That(validationResult.HasErrors, Is.False);
    }

    [Test, TestCaseSource(nameof(Cases))]
    public void RunCase(int number, string message)
    {
      // Arrange
      var writer = Substitute.For<IWriter>();
      var flow = new Flow4
      {
        Number = number,
        Writer = writer
      };

      // Act
      flow.RunAsync().Wait();

      // Assert
      writer.Received().Write(message);
    }

    public static IEnumerable<TestCaseData> Cases
    {
      get
      {
        yield return new TestCaseData(1, "1");
        yield return new TestCaseData(2, "2");
        yield return new TestCaseData(3, "3");
      }
    }
  }
}
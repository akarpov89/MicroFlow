using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;

namespace MicroFlow.Test
{
  [TestFixture]
  public class Flow9Tests
  {
    [Test]
    public void FlowIsValid()
    {
      // Arrange
      var reader = Substitute.For<IReader>();
      var flow = new Flow9 { Reader = reader };

      // Act
      var validationResult = flow.Validate();

      // Assert
      Assert.That(validationResult.HasErrors, Is.False);
    }

    [Test, TestCaseSource(nameof(Cases))]
    public void RunCase(string first, string second, int expectedSum)
    {
      // Arrange
      var reader = new ArrayReader(first, second);
      var flow = new Flow9 {Reader = reader};

      // Act
      var result = flow.RunAsync().Result;

      // Assert
      Assert.That(result, Is.EqualTo(expectedSum));
    }

    public static IEnumerable<TestCaseData> Cases
    {
      get
      {
        yield return Case("1", "2", 3);
        yield return Case("44", "-2", 42);
      }
    }

    private static TestCaseData Case(string first, string second, int result) => new TestCaseData(first, second, result);
  }
}
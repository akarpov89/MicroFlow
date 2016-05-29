using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;

namespace MicroFlow.Test
{
  [TestFixture]
  public abstract class Flow1TestBase
  {
    protected abstract Flow CreateFlow(IReader reader, IWriter writer);

    [Test]
    public void FlowIsValid()
    {
      // Arrange
      var reader = Substitute.For<IReader>();
      var writer = Substitute.For<IWriter>();
      var flow = CreateFlow(reader, writer);

      // Act
      var validationResult = flow.Validate();

      // Assert
      Assert.That(validationResult.HasErrors, Is.False);
    }

    [TestCase("1", "2", "1 <= 2")]
    [TestCase("2", "1", "2 > 1")]
    [TestCase("42", "42", "42 <= 42")]
    public void RunCase(string first, string second, string expectedMessage)
    {
      // Arrange
      var reader = new ArrayReader(first, second);
      var writer = Substitute.For<IWriter>();
      var flow = CreateFlow(reader, writer);

      // Act
      flow.RunAsync().Wait();

      // Assert
      writer.Received().Write(expectedMessage);
    }
  }

  [TestFixture]
  public class Flow1Tests : Flow1TestBase
  {
    protected override Flow CreateFlow(IReader reader, IWriter writer)
    {
      return new Flow1 {Reader = reader, Writer = writer};
    }
  }
}
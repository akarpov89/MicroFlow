using System.Collections.Generic;
using JetBrains.Annotations;
using NSubstitute;
using NUnit.Framework;

namespace MicroFlow.Test
{
  [TestFixture]
  public abstract class Flow3TestsBase
  {
    [NotNull]
    protected abstract Flow CreateFlow([NotNull] IReader reader, [NotNull] IWriter writer);

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

    [TestCase("1", "Echo: 1")]
    [TestCase("42", "Echo: 42")]
    public void RunCase(string inputNumber, string echoMessage)
    {
      // Arrange
      var reader = new ArrayReader(inputNumber);
      var writer = Substitute.For<IWriter>();
      var flow = CreateFlow(reader, writer);

      // Act
      flow.RunAsync().Wait();

      // Assert
      writer.Received().Write(echoMessage);
    }
  }

  [TestFixture]
  public class Flow3Tests : Flow3TestsBase
  {
    protected override Flow CreateFlow(IReader reader, IWriter writer)
    {
      return new Flow3 {Reader = reader, Writer = writer};
    }
  }
}
using System.Collections.Generic;
using JetBrains.Annotations;
using NSubstitute;
using NUnit.Framework;

namespace MicroFlow.Test
{
  [TestFixture]
  public abstract class Flow2TestsBase
  {
    [NotNull]
    protected abstract Flow CreateFlow([NotNull] IWriter writer, int a = 0, int b = 0, int c = 0);

    [Test]
    public void FlowIsValid()
    {
      // Arrange
      var writer = Substitute.For<IWriter>();
      var flow = CreateFlow(writer);

      // Act
      var validationResult = flow.Validate();

      // Assert
      Assert.That(validationResult.HasErrors, Is.False);
    }

    [TestCase(1, 2, 3, "2 + 3 + 4 = 9")]
    [TestCase(0, 8, 19, "1 + 9 + 20 = 30")]
    public void RunCase(int a, int b, int c, string expectedMessage)
    {
      // Arrange
      var writer = Substitute.For<IWriter>();
      var flow = CreateFlow(writer, a, b, c);

      // Act
      flow.RunAsync().Wait();

      // Assert
      writer.Received().Write(expectedMessage);
    }
  }

  [TestFixture]
  public class Flow2Tests : Flow2TestsBase
  {
    protected override Flow CreateFlow(IWriter writer, int a = 0, int b = 0, int c = 0)
    {
      return new Flow2 {Writer = writer, A = a, B = b, C = c};
    }
  }
}
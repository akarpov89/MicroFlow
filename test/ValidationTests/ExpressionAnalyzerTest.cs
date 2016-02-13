using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace MicroFlow.Test
{
  [TestFixture]
  public class ExpressionAnalyzerTest
  {
    [Test]
    public void CorrectlyFindsDependencies()
    {
      // Arrange
      var analyzer = new ExpressionAnalyzer();

      var sourceId = Guid.NewGuid();
      var result = new Result<int>(sourceId);

      Expression<Func<int>> expr = () => result.Get();

      // Act
      analyzer.Visit(expr);

      // Assert
      Assert.That(analyzer.IsValid, Is.True);
      Assert.That(analyzer.Dependencies.Count, Is.EqualTo(1));
      Assert.That(analyzer.Dependencies[0], Is.EqualTo(sourceId));
    }
  }
}
using System;
using NUnit.Framework;

namespace MicroFlow.Test
{
  [TestFixture]
  public class Flow8Tests
  {
    [Test]
    public void ReturnsFaultedTask()
    {
      // Arrange
      var flow = new Flow8();

      // Act
      var task = flow.RunAsync();

      // Assert
      Assert.Throws<AggregateException>(() => task.Wait());
    }
  }
}
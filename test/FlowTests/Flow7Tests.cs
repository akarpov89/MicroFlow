using NUnit.Framework;

namespace MicroFlow.Test
{
  [TestFixture]
  public class Flow7Tests
  {
    [Test]
    public void ReturnsFaultedTask()
    {
      // Arrange
      var flow = new Flow7();

      // Act
      var task = flow.Run();

      // Assert
      Assert.That(task.IsFaulted, Is.True);
    }
  }
}
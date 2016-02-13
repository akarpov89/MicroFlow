using NUnit.Framework;

namespace MicroFlow.Test
{
  [TestFixture]
  public class Flow6Tests
  {
    [Test]
    public void ReturnsFaultedTask()
    {
      // Arrange
      var flow = new Flow6();

      // Act
      var task = flow.Run();

      // Assert
      Assert.That(task.IsFaulted, Is.True);
    }
  }
}
using NUnit.Framework;

namespace MicroFlow.Test
{
    [TestFixture]
    public class Flow5Tests
    {
        [Test]
        public void ReturnsFaultedTask()
        {
            // Arrange
            var flow = new Flow5();

            // Act
            var task = flow.Run();

            // Assert
            Assert.That(task.IsFaulted, Is.True);
        }
    }
}
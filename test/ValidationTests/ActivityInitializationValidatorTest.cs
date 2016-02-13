using NUnit.Framework;

namespace MicroFlow.Test
{
  [TestFixture]
  public class ActivityInitializationValidatorTest
  {
    [Test]
    public void CheckForMultipleInitializers()
    {
      // Arrange
      var b = new FlowBuilder();
      var node = b.Activity<DelayedIncrementActivity>();
      node.Bind(a => a.X).To(1);
      node.Bind(a => a.X).To(42);

      var validator = new ActivityInitializationValidator();

      // Act
      validator.Validate(b.CreateFlow());

      // Assert
      Assert.That(validator.Result.GetErrorsOf(node), Is.Not.Empty);
    }

    [Test]
    public void CheckRequiredInitializers()
    {
      // Arrange
      var b = new FlowBuilder();
      var node = b.Activity<DelayedIncrementActivity>();

      var validator = new ActivityInitializationValidator();

      // Act
      validator.Validate(b.CreateFlow());

      // Assert
      Assert.That(validator.Result.GetErrorsOf(node), Is.Not.Empty);
    }

    [Test]
    public void NoErrorsWhenGood()
    {
      // Arrange
      var b = new FlowBuilder();
      var node = b.Activity<DelayedIncrementActivity>();
      node.Bind(a => a.X).To(1);

      var validator = new ActivityInitializationValidator();

      // Act
      validator.Validate(b.CreateFlow());

      // Assert
      Assert.That(validator.Result.GetErrorsOf(node), Is.Empty);
    }
  }
}
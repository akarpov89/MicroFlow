using System.Threading.Tasks;
using NUnit.Framework;

namespace MicroFlow.Test
{
    [TestFixture]
    public class ActivityTypeValidatorTest
    {
        public abstract class AbstractActivity : Activity
        {
        }

        public class ActivityWithoutPublicCtor : Activity
        {
            private ActivityWithoutPublicCtor() {}

            protected override Task ExecuteCore()
            {
                return null;
            }
        }

        [Test]
        public void ErrorWhenActivityIsAbstract()
        {
            // Arrange
            var builder = new FlowBuilder();
            var node = builder.Activity<AbstractActivity>();
            var validator = new ActivityTypeValidator();

            // Act
            validator.Validate(builder.CreateFlow());

            // Assert
            Assert.That(validator.Result.GetErrorsOf(node), Is.Not.Empty);
        }

        [Test]
        public void ErrorWhenActivityHasNoPublicConstructor()
        {
            // Arrange
            var builder = new FlowBuilder();
            var node = builder.Activity<ActivityWithoutPublicCtor>();
            var validator = new ActivityTypeValidator();

            // Act
            validator.Validate(builder.CreateFlow());

            // Assert
            Assert.That(validator.Result.GetErrorsOf(node), Is.Not.Empty);
        }
    }
}
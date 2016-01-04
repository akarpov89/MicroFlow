using System.Threading.Tasks;

namespace MicroFlow.Test
{
    public class DummyActivity : Activity
    {
        protected override Task ExecuteCore()
        {
            return TaskHelper.CompletedTask;
        }
    }
}
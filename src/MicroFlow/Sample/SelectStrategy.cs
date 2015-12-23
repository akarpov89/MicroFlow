using MicroFlow;

namespace Sample
{
    public class SelectStrategy : SynchronousActivity<Strategy>
    {
        protected override Strategy ExecuteSynchronously()
        {
            return Strategy.UpdateExisting;
        }
    }
}
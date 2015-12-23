using MicroFlow;

namespace Sample
{
    public class SelectStrategy : SyncActivity<Strategy>
    {
        protected override Strategy ExecuteSyncActivity()
        {
            return Strategy.UpdateExisting;
        }
    }
}
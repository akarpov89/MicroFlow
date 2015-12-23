namespace MicroFlow
{
    public abstract class UnitActivity : SyncActivity<Unit>
    {
        protected override Unit ExecuteSyncActivity()
        {
            ExecuteAction();
            return Unit.Instance;
        }

        protected abstract void ExecuteAction();
    }
}
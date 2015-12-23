namespace MicroFlow
{
    public abstract class VoidActivity : SynchronousActivity<Void>
    {
        protected override Void ExecuteSynchronously()
        {
            ExecuteAction();
            return Void.Instance;
        }

        protected abstract void ExecuteAction();
    }
}
using System;
using System.Threading.Tasks;

namespace MicroFlow
{
  public abstract class SyncActivity<TResult> : Activity<TResult>
  {
    public sealed override Task<TResult> Execute()
    {
      try
      {
        TResult result = ExecuteActivity();
        return TaskHelper.FromResult(result);
      }
      catch (Exception ex)
      {
        return TaskHelper.FromException<TResult>(ex);
      }
    }

    protected abstract TResult ExecuteActivity();
  }

  public abstract class SyncActivity : Activity
  {
    protected sealed override Task ExecuteCore()
    {
      try
      {
        ExecuteActivity();
        return TaskHelper.CompletedTask;
      }
      catch (Exception ex)
      {
        return TaskHelper.FromException(ex);
      }
    }

    protected abstract void ExecuteActivity();
  }
}
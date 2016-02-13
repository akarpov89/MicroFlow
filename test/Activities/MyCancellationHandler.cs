using System.Diagnostics;

namespace MicroFlow.Test
{
  public class MyCancellationHandler : SyncActivity
  {
    protected override void ExecuteActivity()
    {
      Debug.WriteLine("Cancelled");
    }
  }
}
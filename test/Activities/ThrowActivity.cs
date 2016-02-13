using System;

namespace MicroFlow.Test
{
  public sealed class ThrowActivity : SyncActivity
  {
    public ThrowActivity()
    {
      throw new Exception("Exception from ThrowActivity::ctor");
    }

    protected override void ExecuteActivity()
    {
    }
  }
}
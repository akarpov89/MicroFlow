using System;

namespace MicroFlow.Test
{
  public class ReadIntActivity : SyncActivity<int>
  {
    private readonly IReader myReader;

    public ReadIntActivity(IReader reader)
    {
      myReader = reader;
    }

    protected override int ExecuteActivity()
    {
      return Convert.ToInt32(myReader.Read());
    }
  }
}
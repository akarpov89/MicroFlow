using System;
using System.Threading.Tasks;

namespace MicroFlow
{
  public abstract class Flow : Flow<Null>
  {
    [Obsolete("Consider using method RunAsync instead of Run")]
    public Task Run()
    {
      return RunAsync();
    }
  }
}
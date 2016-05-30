using MicroFlow.Test;
using NUnit.Framework;

namespace MicroFlow.Meta.Test
{
  [TestFixture]
  public class MetaFlow2Tests : Flow2TestsBase
  {
    protected override Flow CreateFlow(IWriter writer, int a = 0, int b = 0, int c = 0)
    {
      dynamic flow = MetaFlow2.Create();

      flow.Writer = writer;
      flow.A = a;
      flow.B = b;
      flow.C = c;

      return flow;
    }
  }
}
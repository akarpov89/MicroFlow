using MicroFlow.Test;
using NUnit.Framework;

namespace MicroFlow.Meta.Test
{
  [TestFixture]
  public class MetaFlow1Tests : Flow1TestBase
  {
    protected override Flow CreateFlow(IReader reader, IWriter writer)
    {
      dynamic flow = MetaFlow1.Create();
      flow.Reader = reader;
      flow.Writer = writer;

      return flow;
    }
  }
}
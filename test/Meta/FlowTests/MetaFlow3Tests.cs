using MicroFlow.Test;
using NUnit.Framework;

namespace MicroFlow.Meta.Test
{
  [TestFixture]
  public class MetaFlow3Tests : Flow3TestsBase
  {
    protected override Flow CreateFlow(IReader reader, IWriter writer)
    {
      dynamic flow = MetaFlow3.Create();

      flow.Reader = reader;
      flow.Writer = writer;

      return flow;
    }
  }
}
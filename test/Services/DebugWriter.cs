using System.Diagnostics;

namespace MicroFlow.Test
{
  public class DebugWriter : IWriter
  {
    public void Write(string text)
    {
      Debug.WriteLine(text);
    }
  }
}
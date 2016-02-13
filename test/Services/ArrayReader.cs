using System.Collections.Generic;

namespace MicroFlow.Test
{
  public class ArrayReader : IReader
  {
    private readonly Queue<string> myQueue;

    public ArrayReader(params string[] lines)
    {
      myQueue = new Queue<string>(lines);
    }

    public string Read()
    {
      return myQueue.Dequeue();
    }
  }
}
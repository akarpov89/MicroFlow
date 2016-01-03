using System.Collections.Generic;

namespace MicroFlow.Test
{
    public class ArrayReader : IReader
    {
        private readonly Queue<string> _queue;

        public ArrayReader(params string[] lines)
        {
            _queue = new Queue<string>(lines);
        }

        public string Read()
        {
            return _queue.Dequeue();
        }
    }
}
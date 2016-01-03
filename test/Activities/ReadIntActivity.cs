using System;

namespace MicroFlow.Test
{
    public class ReadIntActivity : SyncActivity<int>
    {
        private readonly IReader _reader;

        public ReadIntActivity(IReader reader)
        {
            _reader = reader;
        }

        protected override int ExecuteActivity()
        {
            return Convert.ToInt32(_reader.Read());
        }
    }
}
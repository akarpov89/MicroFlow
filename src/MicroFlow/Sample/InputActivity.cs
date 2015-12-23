using System;
using MicroFlow;

namespace Sample
{
    internal class InputActivity : SynchronousActivity<int>
    {
        private readonly IReadService _readService;
        private readonly IWriteService _writeService;

        public InputActivity(IReadService readService, IWriteService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        protected override int ExecuteSynchronously()
        {
            _writeService.Write("Input number: ");
            return Convert.ToInt32(_readService.Read());
        }
    }
}
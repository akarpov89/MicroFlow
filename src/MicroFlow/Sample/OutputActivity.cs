using MicroFlow;

namespace Sample
{
    internal class OutputActivity : SyncActivity
    {
        private readonly IWriteService _writeService;

        public OutputActivity(IWriteService writeService)
        {
            _writeService = writeService;
        }

        public string Message { get; set; }

        protected override void ExecuteActivity()
        {
            _writeService.Write(Message);
        }
    }
}
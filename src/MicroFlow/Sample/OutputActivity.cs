using MicroFlow;

namespace Sample
{
    internal class OutputActivity : SequentialActivity
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
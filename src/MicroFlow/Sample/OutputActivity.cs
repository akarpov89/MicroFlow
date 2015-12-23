using MicroFlow;

namespace Sample
{
    internal class OutputActivity : VoidActivity
    {
        private readonly IWriteService _writeService;

        public OutputActivity(IWriteService writeService)
        {
            _writeService = writeService;
        }

        public string Message { get; set; }

        protected override void ExecuteAction()
        {
            _writeService.Write(Message);
        }
    }
}
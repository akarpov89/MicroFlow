namespace MicroFlow.Test
{
    public class WriteMessageActivity : SyncActivity
    {
        private readonly IWriter _writer;

        public WriteMessageActivity(IWriter writer)
        {
            _writer = writer;
        }

        [Required]
        public string Message { get; set; }

        protected override void ExecuteActivity()
        {
            _writer.Write(Message);
        }
    }
}
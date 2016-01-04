namespace MicroFlow.Test
{
    public class SumActivity : SyncActivity
    {
        private readonly IWriter _writer;

        public SumActivity(IWriter writer)
        {
            _writer = writer;
        }

        [Required]
        public int A { get; set; }

        [Required]
        public int B { get; set; }

        [Required]
        public int C { get; set; }

        protected override void ExecuteActivity()
        {
            _writer.Write($"{A} + {B} + {C} = {A + B + C}");
        }
    }
}
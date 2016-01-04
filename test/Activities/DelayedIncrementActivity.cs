using System.Threading;
using System.Threading.Tasks;

namespace MicroFlow.Test
{
    public class DelayedIncrementActivity : Activity<int>
    {
        [Required]
        public int X { get; set; }

        public override Task<int> Execute()
        {
            return Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
                return X + 1;
            });
        }
    }
}
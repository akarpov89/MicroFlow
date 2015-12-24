using System;
using MicroFlow;

namespace Sample
{
    public class MyCancellationHandler : SequentialActivity
    {
        protected override void ExecuteActivity()
        {
            Console.WriteLine("Cancellation handler!");
        }
    }
}
using System;
using MicroFlow;

namespace Sample
{
    public class MyCancellationHandler : SyncActivity
    {
        protected override void ExecuteActivity()
        {
            Console.WriteLine("Cancellation handler!");
        }
    }
}
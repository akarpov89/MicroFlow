using System;
using MicroFlow;

namespace Sample
{
    public class MyCancellationHandler : UnitActivity
    {
        protected override void ExecuteAction()
        {
            Console.WriteLine("Cancellation handler!");
        }
    }
}
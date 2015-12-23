using System;
using MicroFlow;

namespace Sample
{
    public class MyCancellationHandler : VoidActivity
    {
        protected override void ExecuteAction()
        {
            Console.WriteLine("Cancellation handler!");
        }
    }
}
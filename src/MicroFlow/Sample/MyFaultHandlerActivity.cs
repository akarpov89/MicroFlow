using System;
using System.Linq;
using MicroFlow;

namespace Sample
{
    public class MyFaultHandlerActivity : SyncActivity, IFaultHandlerActivity
    {
        public Exception Exception { get; set; }

        protected override void ExecuteActivity()
        {
            var aggregateException = Exception as AggregateException;
            if (aggregateException != null)
            {
                Exception first = aggregateException.InnerExceptions.First();
                Console.WriteLine(first.Message);
            }
            else
            {
                Console.WriteLine(Exception.Message);
            }
        }
    }
}
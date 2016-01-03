using System;
using System.Diagnostics;
using System.Linq;

namespace MicroFlow.Test
{
    public class MyFaultHandler : SyncActivity, IFaultHandlerActivity
    {
        public Exception Exception { get; set; }

        protected override void ExecuteActivity()
        {
            var aggregateException = Exception as AggregateException;
            if (aggregateException != null)
            {
                Exception first = aggregateException.InnerExceptions.First();
                Debug.WriteLine(first.Message);
            }
            else
            {
                Debug.WriteLine(Exception.Message);
            }
        }
    }
}
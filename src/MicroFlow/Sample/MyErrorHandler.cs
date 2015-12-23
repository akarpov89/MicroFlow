using System;
using System.Linq;
using MicroFlow;

namespace Sample
{
    public class MyErrorHandler : VoidActivity, IErrorHandler
    {
        public Exception Exception { get; set; }

        protected override void ExecuteAction()
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
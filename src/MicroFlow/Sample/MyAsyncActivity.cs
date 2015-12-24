using System;
using System.Threading;
using System.Threading.Tasks;
using MicroFlow;

namespace Sample
{
    public class MyAsyncActivity : Activity<int>
    {
        public int X { get; set; }

        public override Task<int> Execute()
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Fork started. X = {0}. ThreadId = {1}", X, Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(2000);
                X = X + 1;
                Console.WriteLine("Fork ending. X = {0}. ThreadId = {1}", X, Thread.CurrentThread.ManagedThreadId);
                return X;
            });
        }
    }
}
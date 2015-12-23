using System;

namespace Sample
{
    internal class ConsoleReadService : IReadService
    {
        public ConsoleReadService()
        {
            Console.WriteLine("ctor ConsoleReadService");
        }

        public string Read() => Console.ReadLine();
    }
}
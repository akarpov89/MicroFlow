using System;

namespace Sample
{
    internal class ConsoleWriteService : IWriteService
    {
        public ConsoleWriteService()
        {
            Console.WriteLine("ctor ConsoleWriteService");
        }

        public void Write(string message) => Console.Write(message);
    }
}
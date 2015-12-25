using System;
using MicroFlow;

namespace Sample
{
    public class MyAsyncSum : SyncActivity
    {
        [Required]
        public int A { get; set; }

        [Required]
        public int B { get; set; }

        [Required]
        public int C { get; set; }

        protected override void ExecuteActivity()
        {
            Console.WriteLine($"{A} + {B} + {C} = {A + B + C}");
        }
    }
}
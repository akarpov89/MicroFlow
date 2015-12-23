using System;
using MicroFlow;

namespace Sample
{
    public class MyAsyncSum : UnitActivity
    {
        [Required]
        public int A { get; set; }

        [Required]
        public int B { get; set; }

        [Required]
        public int C { get; set; }

        protected override void ExecuteAction()
        {
            Console.WriteLine($"{A} + {B} + {C} = {A + B + C}");
        }
    }
}
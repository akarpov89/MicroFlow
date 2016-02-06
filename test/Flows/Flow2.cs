namespace MicroFlow.Test
{
    public class Flow2 : Flow
    {
        public IWriter Writer { get; set; }

        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }

        public override string Name => "Flow2. Uses fork-join node";

        protected override void Build(FlowBuilder builder)
        {
            builder.WithDefaultFaultHandler<MyFaultHandler>();
            builder.WithDefaultCancellationHandler<MyCancellationHandler>();

            var forkJoin = builder.ForkJoin("My fork join node");

            var first = forkJoin.Fork<DelayedIncrementActivity>("First fork");
            first.Bind(a => a.X).To(A);

            var second = forkJoin.Fork<DelayedIncrementActivity>("Second fork");
            second.Bind(a => a.X).To(B);

            var third = forkJoin.Fork<DelayedIncrementActivity>("Third fork");
            third.Bind(a => a.X).To(C);

            var sum = builder.Activity<SumActivity>("Sum action");
            sum.Bind(x => x.A).ToResultOf(first);
            sum.Bind(x => x.B).ToResultOf(second);
            sum.Bind(x => x.C).ToResultOf(third);

            builder.WithInitialNode(forkJoin);

            forkJoin.ConnectTo(sum);
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IWriter>(Writer);
        }
    }
}
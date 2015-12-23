using System;
using JetBrains.Annotations;

namespace MicroFlow
{
    public delegate T Updater<T>(T currentValue, T value);

    public interface IVariable
    {
        void RemoveBindings();
    }

    public sealed class Variable<T> : IVariable
    {
        internal Variable(T initialValue = default(T))
        {
            CurrentValue = initialValue;
        }

        public T CurrentValue { get; private set; }

        public void RemoveBindings()
        {
            Action handler = OnRemoveBindings;
            if (handler != null)
            {
                handler();
            }
        }

        private event Action OnRemoveBindings;

        public void Assign(T value)
        {
            CurrentValue = value;
        }

        public void BindToResultOf<TActivity>(ActivityNode<TActivity> node)
            where TActivity : class, IActivity<T>
        {
            AfterCompletionOf(node).AssignActivityResult();
        }

        public AfterCompletionUpdater<T> AfterCompletionOf<TActivity>([NotNull] ActivityNode<TActivity> node)
            where TActivity : class, IActivity<T>
        {
            node.AssertNotNull("node != null");

            var updater = new AfterCompletionUpdater<T>(this, Result<T>.Of(node));
            OnRemoveBindings += () => updater.RemoveBinding();
            return updater;
        }

        public AfterCompletionUpdater<T> AfterCompletionOf<TActivity>([NotNull] ActivityDescriptor<TActivity> descriptor)
            where TActivity : class, IActivity<T>
        {
            descriptor.AssertNotNull("descriptor != null");

            var updater = new AfterCompletionUpdater<T>(this, Result<T>.Of(descriptor));
            OnRemoveBindings += () => updater.RemoveBinding();
            return updater;
        }
    }
}
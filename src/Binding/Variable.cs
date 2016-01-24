using System;
using JetBrains.Annotations;

namespace MicroFlow
{
    public interface IVariable
    {
    }

    public sealed class Variable<T> : IVariable
    {
        internal Variable(T initialValue = default(T))
        {
            CurrentValue = initialValue;
        }

        public T CurrentValue { get; private set; }

        internal void Assign(T value)
        {
            CurrentValue = value;
        }

        internal void Update([NotNull] Func<T, T> updateFunc)
        {
            CurrentValue = updateFunc(CurrentValue);
        }

        internal void Update<TOther>([NotNull] Func<T, TOther, T> updateFunc, TOther otherValue)
        {
            CurrentValue = updateFunc(CurrentValue, otherValue);
        }
    }
}
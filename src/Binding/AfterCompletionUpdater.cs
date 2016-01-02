using System;
using JetBrains.Annotations;

namespace MicroFlow
{
    public sealed class AfterCompletionUpdater<T>
    {
        private readonly Result<T> _result;
        private readonly Variable<T> _variable;

        private Action<T> _resultHandler;

        public AfterCompletionUpdater([NotNull] Variable<T> variable, [NotNull] Result<T> activityResult)
        {
            _variable = variable.NotNull();
            _result = activityResult.NotNull();
        }

        public void AssignActivityResult()
        {
            _resultHandler = result => _variable.Assign(result);
            _result.RegisterResultHandler(_resultHandler);
        }

        public void Assign(T value)
        {
            _resultHandler = _ => _variable.Assign(value);
            _result.RegisterResultHandler(_resultHandler);
        }

        public void Update([NotNull] Updater<T> updater)
        {
            updater.AssertNotNull("updater != null");

            _resultHandler = result => _variable.Assign(updater(_variable.CurrentValue, result));
            _result.RegisterResultHandler(_resultHandler);
        }

        internal void RemoveBinding()
        {
            if (_resultHandler != null)
            {
                _result.UnregisterResultHandler(_resultHandler);
            }
        }
    }
}
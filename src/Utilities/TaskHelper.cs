using System;
#if !PORTABLE
using System.Collections.Generic;
#endif
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MicroFlow
{
    internal static class TaskHelper
    {
        private static Task s_completedTask;
        private static Task s_cancelledTask;

        [NotNull]
        public static Task CompletedTask
        {
            get
            {
                if (s_completedTask == null)
                {
                    s_completedTask = FromResult(default(VoidTaskResult));
                }
                return s_completedTask;
            }
        }

        [NotNull]
        public static Task CancelledTask
        {
            get
            {
                if (s_cancelledTask == null)
                {
                    var tcs = new TaskCompletionSource<VoidTaskResult>();
                    tcs.TrySetCanceled();
                    s_cancelledTask = tcs.Task;
                }
                return s_cancelledTask;
            }
        }

        public static Task<TBase> Convert<TDerived, TBase>(this Task<TDerived> task) where TDerived : TBase
        {
            var tcs = new TaskCompletionSource<TBase>();

            task.ContinueWith(t =>
            {
                if (t.IsCanceled)
                {
                    tcs.TrySetCanceled();
                }
                else if (t.IsFaulted)
                {
                    Debug.Assert(t.Exception != null, "t.Exception != null");
                    tcs.TrySetException(t.Exception.InnerExceptions);
                }
                else
                {
                    tcs.TrySetResult(t.Result);
                }
            }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        public static Task<Null> ToTaskOfNull(this Task task)
        {
            var tcs = new TaskCompletionSource<Null>();

            task.ContinueWith(t =>
            {
                if (t.IsCanceled)
                {
                    tcs.TrySetCanceled();
                }
                else if (t.IsFaulted)
                {
                    Debug.Assert(t.Exception != null, "t.Exception != null");
                    tcs.TrySetException(t.Exception.InnerExceptions);
                }
                else
                {
                    tcs.TrySetResult(null);
                }
            }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        public static Task<TResult> Cancelled<TResult>()
        {
            var tcs = new TaskCompletionSource<TResult>();
            tcs.TrySetCanceled();
            return tcs.Task;
        }

        [NotNull]
        public static Task<TResult> FromResult<TResult>(TResult value)
        {
#if PORTABLE
            return Task.FromResult(value);
#else
            var tcs = new TaskCompletionSource<TResult>();
            tcs.SetResult(value);
            return tcs.Task;
#endif
        }

        [NotNull]
        public static Task<TResult> FromException<TResult>([NotNull] Exception exception)
        {
            var tcs = new TaskCompletionSource<TResult>();
            tcs.SetException(exception);
            return tcs.Task;
        }

        [NotNull]
        public static Task FromException([NotNull] Exception exception)
        {
            var tcs = new TaskCompletionSource<VoidTaskResult>();
            tcs.SetException(exception);
            return tcs.Task;
        }

        [NotNull]
        public static Task<T[]> WhenAll<T>(this Task<T>[] tasks)
        {
#if PORTABLE
            return Task.WhenAll(tasks);
#else
            var tcs = new TaskCompletionSource<T[]>();

            Task.Factory.ContinueWhenAll(tasks, completedTasks =>
            {
                var exceptions = new List<Exception>();
                var results = new T[completedTasks.Length];

                bool hasFaults = false;
                bool hasCancellations = false;

                for (int i = 0; i < completedTasks.Length; ++i)
                {
                    Task<T> task = completedTasks[i];

                    if (task.IsFaulted)
                    {
                        Debug.Assert(task.Exception != null);
                        exceptions.AddRange(task.Exception.InnerExceptions);
                        hasFaults = true;
                    }
                    else if (task.IsCanceled)
                    {
                        hasCancellations = true;
                    }
                    else
                    {
                        results[i] = task.Result;
                    }
                }

                if (hasFaults)
                {
                    tcs.TrySetException(exceptions);
                }
                else if (hasCancellations)
                {
                    tcs.TrySetCanceled();
                }
                else
                {
                    tcs.TrySetResult(results);
                }
            }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
#endif
        }

        [StructLayout(LayoutKind.Auto)]
        private struct VoidTaskResult
        {
        }
    }
}
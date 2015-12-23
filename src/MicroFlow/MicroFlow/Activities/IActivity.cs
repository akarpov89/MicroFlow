using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MicroFlow
{
    public interface IActivity<TResult> : IActivity
    {
        [NotNull]
        new Task<TResult> Execute();
    }

    public interface IActivity
    {
        [NotNull]
        Task<object> Execute();
    }
}
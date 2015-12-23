using System;

namespace MicroFlow
{
    public interface IErrorHandler : IActivity<Unit>
    {
        Exception Exception { get; set; }
    }
}
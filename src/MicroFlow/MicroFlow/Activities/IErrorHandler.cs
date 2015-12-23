using System;

namespace MicroFlow
{
    public interface IErrorHandler : IActivity<Void>
    {
        Exception Exception { get; set; }
    }
}
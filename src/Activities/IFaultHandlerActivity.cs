using System;

namespace MicroFlow
{
    public interface IFaultHandlerActivity : IActivity
    {
        Exception Exception { get; set; }
    }
}
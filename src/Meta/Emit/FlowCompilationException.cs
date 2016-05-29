using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace MicroFlow.Meta
{
  [DataContract]
  public class FlowCompilationException : Exception
  {
    public FlowCompilationException()
    {
    }

    public FlowCompilationException(string message) : base(message)
    {
    }

    public FlowCompilationException([NotNull] string[] errors)
    {
      Errors = errors;
    }

    public FlowCompilationException(string message, Exception inner) : base(message, inner)
    {
    }

    [CanBeNull]
    public string[] Errors { get; }
  }
}
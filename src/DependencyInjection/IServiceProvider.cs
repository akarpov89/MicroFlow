using System;
using JetBrains.Annotations;

namespace MicroFlow
{
  public interface IServiceProvider
  {
    bool HasService([NotNull] Type serviceType);

    [CanBeNull]
    object GetService([NotNull] Type serviceType);

    void ReleaseService([CanBeNull] object service);
  }
}
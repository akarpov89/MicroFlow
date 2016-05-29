using System;
using JetBrains.Annotations;

namespace MicroFlow.Meta
{
  public class ServiceInfo
  {
    public ServiceInfo(
      [NotNull] Type interfaceType, [CanBeNull] Type implementationType = null,
      LifetimeKind lifetimeKind = LifetimeKind.Transient,
      [CanBeNull] string[] constructorArgumentExpressions = null,
      [CanBeNull] string instanceExpression = null)
    {
      Interface = interfaceType.NotNull();
      Implementation = implementationType;
      LifetimeKind = lifetimeKind;
      ConstructorArgumentExpressions = constructorArgumentExpressions;
      InstanceExpression = instanceExpression;
    }

    public Type Interface { get; }

    [CanBeNull]
    public Type Implementation { get; }

    public LifetimeKind LifetimeKind { get; }

    [CanBeNull]
    public string[] ConstructorArgumentExpressions { get; }

    [CanBeNull]
    public string InstanceExpression { get; }
  }

  public enum LifetimeKind
  {
    Transient,
    Singleton,
    DisposableSingleton
  }
}
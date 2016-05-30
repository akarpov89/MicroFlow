using System;
using JetBrains.Annotations;

namespace MicroFlow.Meta
{
  public class ServiceInfo
  {
    public ServiceInfo(
      [NotNull] Type serviceType, [CanBeNull] Type implementationType = null,
      LifetimeKind lifetimeKind = LifetimeKind.Transient,
      [CanBeNull] string[] constructorArgumentExpressions = null,
      [CanBeNull] string instanceExpression = null)
    {
      ServiceType = serviceType.NotNull();
      ImplementationType = implementationType;
      LifetimeKind = lifetimeKind;
      ConstructorArgumentExpressions = constructorArgumentExpressions;
      InstanceExpression = instanceExpression;
    }

    [NotNull]
    public static ServiceInfo Transient(
      [NotNull] Type serviceType, [NotNull] Type implementationType,
      [CanBeNull] string[] constructorArgumentExpressions = null)
    {
      return new ServiceInfo(
        serviceType.NotNull(), implementationType.NotNull(),
        constructorArgumentExpressions: constructorArgumentExpressions);
    }

    [NotNull]
    public static ServiceInfo Transient<TService, TImplementation>(
      [CanBeNull] string[] constructorArgumentExpressions = null)
    {
      return Transient(
        typeof(TService), typeof(TImplementation),
        constructorArgumentExpressions);
    }

    [NotNull]
    public static ServiceInfo Singleton(
      [NotNull] Type serviceType, [NotNull] Type implementationType,
      bool isDisposable = false, [CanBeNull] string[] constructorArgumentExpressions = null)
    {
      return new ServiceInfo(
        serviceType.NotNull(), implementationType.NotNull(),
        isDisposable ? LifetimeKind.DisposableSingleton : LifetimeKind.Singleton,
        constructorArgumentExpressions);
    }

    [NotNull]
    public static ServiceInfo Singleton<TService, TImplementation>(
      bool isDisposable = false, [CanBeNull] string[] constructorArgumentExpressions = null)
    {
      return Singleton(
        typeof(TService), typeof(TImplementation),
        isDisposable, constructorArgumentExpressions);
    }

    [NotNull]
    public static ServiceInfo Singleton(
      [NotNull] Type serviceType, 
      [NotNull] string instanceExpression, 
      bool isDisposable = false)
    {
      return new ServiceInfo(
        serviceType, 
        lifetimeKind: isDisposable ? LifetimeKind.DisposableSingleton : LifetimeKind.Singleton,
        instanceExpression: instanceExpression);
    }

    [NotNull]
    public static ServiceInfo Singleton<TService>(
      [NotNull] string instanceExpression,
      bool isDisposable = false)
    {
      return Singleton(typeof(TService), instanceExpression, isDisposable);
    }

    public Type ServiceType { get; }

    [CanBeNull]
    public Type ImplementationType { get; }

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
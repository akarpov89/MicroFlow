using System;
using JetBrains.Annotations;

namespace MicroFlow
{
    public class ServiceDescriptor
    {
        public ServiceLifetime Lifetime { get; private set; }
        public Type ServiceType { get; private set; }
        public Func<object> Factory { get; private set; }
        public object ImplementationInstance { get; private set; }
        public bool ShouldBeDisposed { get; private set; }

        public static ServiceDescriptor Singleton<TService, TImplementation>()
            where TImplementation : class, TService, new()
        {
            return new ServiceDescriptor
            {
                ServiceType = typeof (TService),
                Factory = () => new TImplementation(),
                Lifetime = ServiceLifetime.Singleton,
                ShouldBeDisposed = typeof (TImplementation).IsDisposableType()
            };
        }

        public static ServiceDescriptor Singleton<TService, TImplementation>([NotNull] TImplementation instance)
            where TImplementation : class, TService
        {
            instance.AssertNotNull("instance != null");

            return new ServiceDescriptor
            {
                ServiceType = typeof (TService),
                ImplementationInstance = instance,
                Lifetime = ServiceLifetime.Singleton,
                ShouldBeDisposed = false
            };
        }

        public static ServiceDescriptor Singleton<TService>([NotNull] object instance)
        {
            instance.AssertNotNull("instance != null");
            (instance is TService).AssertTrue("Instance doesn't impelement service " + typeof(TService));

            return new ServiceDescriptor
            {
                ServiceType = typeof(TService),
                ImplementationInstance = instance,
                Lifetime = ServiceLifetime.Singleton,
                ShouldBeDisposed = false
            };
        }

        public static ServiceDescriptor DisposableSingleton<TService>([NotNull] IDisposable instance)
        {
            instance.AssertNotNull("instance != null");
            (instance is TService).AssertTrue("Instance doesn't impelement service " + typeof(TService));

            return new ServiceDescriptor
            {
                ServiceType = typeof(TService),
                ImplementationInstance = instance,
                Lifetime = ServiceLifetime.Singleton,
                ShouldBeDisposed = true
            };
        }

        public static ServiceDescriptor Transient<TService, TImplementation>()
            where TImplementation : class, TService, new()
        {
            return new ServiceDescriptor
            {
                ServiceType = typeof (TService),
                Factory = () => new TImplementation(),
                Lifetime = ServiceLifetime.Transient,
                ShouldBeDisposed = typeof (TImplementation).IsDisposableType()
            };
        }

        public object GetInstance()
        {
            if (ImplementationInstance != null) return ImplementationInstance;

            if (Lifetime == ServiceLifetime.Singleton)
            {
                ImplementationInstance = Factory();
                return ImplementationInstance;
            }

            return Factory();
        }
    }
}
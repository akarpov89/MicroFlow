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

        public static ServiceDescriptor Singleton<TService, TImplementation>()
            where TImplementation : class, TService, new()
        {
            return new ServiceDescriptor
            {
                ServiceType = typeof (TService),
                Factory = () => new TImplementation(),
                Lifetime = ServiceLifetime.Singleton
            };
        }

        public static ServiceDescriptor Singleton<TService, TImplementation>([NotNull] TImplementation instance)
            where TImplementation : class, TService, new()
        {
            instance.AssertNotNull("instance != null");

            return new ServiceDescriptor
            {
                ServiceType = typeof (TService),
                ImplementationInstance = instance,
                Lifetime = ServiceLifetime.Singleton
            };
        }

        public static ServiceDescriptor Transient<TService, TImplementation>()
            where TImplementation : class, TService, new()
        {
            return new ServiceDescriptor
            {
                ServiceType = typeof (TService),
                Factory = () => new TImplementation(),
                Lifetime = ServiceLifetime.Transient
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
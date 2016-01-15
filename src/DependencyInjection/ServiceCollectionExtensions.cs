using System;
using JetBrains.Annotations;

namespace MicroFlow
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSingleton<TService, TImplementation>(
            [NotNull] this IServiceCollection collection)
            where TImplementation : class, TService, new()
        {
            collection.Add(ServiceDescriptor.Singleton<TService, TImplementation>());
            return collection;
        }

        public static IServiceCollection AddSingleton<TService, TImplementation>(
            [NotNull] this IServiceCollection collection, [NotNull] TImplementation instance)
            where TImplementation : class, TService
        {
            collection.Add(ServiceDescriptor.Singleton<TService, TImplementation>(instance));
            return collection;
        }

        public static IServiceCollection AddSingleton<TService>(
            [NotNull] this IServiceCollection collection, [NotNull] object instance)
        {
            collection.Add(ServiceDescriptor.Singleton<TService>(instance));
            return collection;
        }

        public static IServiceCollection AddDisposableSingleton<TService>(
            [NotNull] this IServiceCollection collection, [NotNull] IDisposable instance)
        {
            collection.Add(ServiceDescriptor.DisposableSingleton<TService>(instance));
            return collection;
        }

        public static IServiceCollection AddTransient<TService, TImplementation>(
            [NotNull] this IServiceCollection collection)
            where TImplementation : class, TService, new()
        {
            collection.Add(ServiceDescriptor.Transient<TService, TImplementation>());
            return collection;
        }
    }
}
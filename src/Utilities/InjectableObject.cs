using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace MicroFlow
{
    internal struct InjectableObject<T> : IDisposable where T : class
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly T _instance;
        private readonly object[] _services;

        private bool _isDisposed;

        internal InjectableObject([NotNull] T instance, [NotNull] IServiceProvider serviceProvider, object[] services)
        {
            _instance = instance.NotNull();
            _serviceProvider = serviceProvider.NotNull();
            _services = services;
            _isDisposed = false;
        }

        public T Instance
        {
            get
            {
                if (_isDisposed) throw new ObjectDisposedException(ToString());
                return _instance;
            }
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                if (_services != null && _services.Length > 0)
                {
                    foreach (object service in _services)
                    {
                        _serviceProvider.ReleaseService(service);
                    }
                }

                var disposable = _instance as IDisposable;
                disposable?.Dispose();

                _isDisposed = true;
            }
        }

        public static InjectableObject<T> Create([NotNull] IServiceProvider serviceProvider)
        {
            serviceProvider.AssertNotNull("serviceProvider != null");

            object[] services;
            object instance = CreateInstance(typeof (T), serviceProvider, out services);

            return new InjectableObject<T>((T) instance, serviceProvider, services);
        }

        public static InjectableObject<T> Create([NotNull] Type type, [NotNull] IServiceProvider serviceProvider)
        {
            type.AssertNotNull("type != null");
            serviceProvider.AssertNotNull("serviceProvider != null");

#if PORTABLE
            Debug.Assert(typeof (T).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()));
#else
            Debug.Assert(typeof (T).IsAssignableFrom(type));
#endif

            object[] services;
            object instance = CreateInstance(type, serviceProvider, out services);

            return new InjectableObject<T>((T) instance, serviceProvider, services);
        }

        private static object CreateInstance(Type type, IServiceProvider serviceProvider, out object[] parameters)
        {
#if PORTABLE
            ConstructorInfo[] constructors = type.GetTypeInfo().DeclaredConstructors.ToArray();
#else
            ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
#endif

            Array.Sort(constructors, new ByParametersCountComparer());

            if (constructors[0].GetParameters().Length == 0)
            {
                Func<object> factory = TypeUtils.CreateDefaultConstructorFactoryFor(type);
                parameters = null;
                return factory();
            }

            ConstructorInfo applicableConstuctor = FindApplicableConstructor(constructors, serviceProvider);
            if (applicableConstuctor == null) throw new InvalidOperationException("Applicable constuctor not found");

            parameters = CreateParameters(applicableConstuctor, serviceProvider);
            return applicableConstuctor.Invoke(parameters);
        }

        [CanBeNull]
        private static ConstructorInfo FindApplicableConstructor(
            ConstructorInfo[] constructors, IServiceProvider serviceProvider)
        {
            foreach (ConstructorInfo constructor in constructors)
            {
                ParameterInfo[] parameters = constructor.GetParameters();

                bool isApplicable = parameters.All(parameter => serviceProvider.HasService(parameter.ParameterType));

                if (isApplicable) return constructor;
            }

            return null;
        }

        [NotNull]
        private static object[] CreateParameters(ConstructorInfo constructor, IServiceProvider serviceProvider)
        {
            ParameterInfo[] parameters = constructor.GetParameters();
            var result = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; ++i)
            {
                result[i] = serviceProvider.GetService(parameters[i].ParameterType);
            }

            return result;
        }

        private class ByParametersCountComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                var left = (ConstructorInfo) x;
                var right = (ConstructorInfo) y;

                return left.GetParameters().Length.CompareTo(right.GetParameters().Length);
            }
        }
    }
}
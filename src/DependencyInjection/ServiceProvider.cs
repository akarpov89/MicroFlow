using System;
using System.Linq;
using JetBrains.Annotations;

namespace MicroFlow
{
    internal sealed class ServiceProvider : IServiceProvider, IDisposable
    {
        private readonly IServiceCollection _services;
        private bool _isDisposed;

        public ServiceProvider([NotNull] IServiceCollection services)
        {
            _services = services.NotNull();
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                foreach (ServiceDescriptor serviceDescriptor in _services)
                {
                    if (serviceDescriptor.ShouldBeDisposed)
                        DisposeService(serviceDescriptor.ImplementationInstance);
                }

                _services.Clear();
                _isDisposed = true;
            }
        }

        public bool HasService(Type serviceType)
        {
            AssertNotDisposed();
            serviceType.AssertNotNull("serviceType != null");

            return FindServiceDescriptor(serviceType) != null;
        }

        public object GetService(Type serviceType)
        {
            AssertNotDisposed();
            serviceType.AssertNotNull("serviceType != null");

            ServiceDescriptor serviceDescriptor = FindServiceDescriptor(serviceType);

            return serviceDescriptor?.GetInstance();
        }

        public void ReleaseService(object service)
        {
            AssertNotDisposed();

            if (service == null) return;

            ServiceDescriptor serviceDescriptor = FindServiceDescriptor(service.GetType());
            if (serviceDescriptor == null || serviceDescriptor.Lifetime == ServiceLifetime.Singleton) return;

            DisposeService(service);
        }

        [CanBeNull]
        private ServiceDescriptor FindServiceDescriptor(Type serviceType)
        {
            return _services.FirstOrDefault(serviceDescriptor => serviceDescriptor.ServiceType == serviceType);
        }

        private void DisposeService([CanBeNull] object service)
        {
            var disposable = service as IDisposable;
            disposable?.Dispose();
        }

        private void AssertNotDisposed()
        {
            if (_isDisposed) throw new ObjectDisposedException(typeof(ServiceProvider).FullName);
        }
    }
}
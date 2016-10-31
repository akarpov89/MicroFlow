using System;
using System.Linq;
using JetBrains.Annotations;

namespace MicroFlow
{
  internal sealed class ServiceProvider : IServiceProvider, IDisposable
  {
    private readonly IServiceCollection myServices;
    private bool myIsDisposed;

    public ServiceProvider([NotNull] IServiceCollection services)
    {
      myServices = services.NotNull();
    }

    public void Dispose()
    {
      if (!myIsDisposed)
      {
        foreach (ServiceDescriptor serviceDescriptor in myServices)
        {
          if (serviceDescriptor.ShouldBeDisposed)
            DisposeService(serviceDescriptor.ImplementationInstance);
        }

        myServices.Clear();
        myIsDisposed = true;
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
      return myServices.FirstOrDefault(serviceDescriptor => serviceType.Is(serviceDescriptor.ServiceType));
    }

    private void DisposeService([CanBeNull] object service)
    {
      var disposable = service as IDisposable;
      disposable?.Dispose();
    }

    private void AssertNotDisposed()
    {
      if (myIsDisposed) throw new ObjectDisposedException(typeof (ServiceProvider).FullName);
    }
  }
}
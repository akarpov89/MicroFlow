using System;
using JetBrains.Annotations;

namespace MicroFlow
{
  internal struct InjectableObjectsCollection<T> : IDisposable where T : class
  {
    private readonly InjectableObject<T>[] myObjects;
    private int myCount;

    public InjectableObjectsCollection(int count)
    {
      myObjects = new InjectableObject<T>[count];
      myCount = 0;
    }

    public T Add(Type type, IServiceProvider serviceProvider)
    {
      (myCount != myObjects.Length).AssertTrue("Array of injectable objects is already full");

      InjectableObject<T> injectableObject = InjectableObject<T>.Create(type, serviceProvider);
      myObjects[myCount++] = injectableObject;

      return injectableObject.Instance;
    }

    public TOutput[] ConvertInstances<TOutput>([NotNull] Func<T, TOutput> func)
    {
      func.AssertNotNull("func != null");

      var result = new TOutput[myObjects.Length];

      for (int i = 0; i < myObjects.Length; ++i)
      {
        result[i] = func(myObjects[i].Instance);
      }

      return result;
    }

    public void Dispose()
    {
      foreach (InjectableObject<T> injectableObject in myObjects)
      {
        injectableObject.Dispose();
      }
    }
  }
}
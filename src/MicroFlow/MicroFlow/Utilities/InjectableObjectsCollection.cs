using System;
using JetBrains.Annotations;

namespace MicroFlow
{
    internal struct InjectableObjectsCollection<T> : IDisposable where T : class
    {
        private readonly InjectableObject<T>[] _objects;
        private int _count;

        public InjectableObjectsCollection(int count)
        {
            _objects = new InjectableObject<T>[count];
            _count = 0;
        }

        public T Add(Type type, IServiceProvider serviceProvider)
        {
            (_count != _objects.Length).AssertTrue("Array of injectable objects is already full");

            InjectableObject<T> injectableObject = InjectableObject<T>.Create(type, serviceProvider);
            _objects[_count++] = injectableObject;

            return injectableObject.Instance;
        }

        public TOutput[] ConvertInstances<TOutput>([NotNull] Func<T, TOutput> func)
        {
            func.AssertNotNull("func != null");

            var result = new TOutput[_objects.Length];

            for (int i = 0; i < _objects.Length; ++i)
            {
                result[i] = func(_objects[i].Instance);
            }

            return result;
        }

        public void Dispose()
        {
            foreach (InjectableObject<T> injectableObject in _objects)
            {
                injectableObject.Dispose();
            }
        }
    }
}
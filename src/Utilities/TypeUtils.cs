using System;
using System.Collections.Generic;
using System.Reflection;
#if !PORTABLE
using System.Reflection.Emit;
#endif
using JetBrains.Annotations;

namespace MicroFlow
{
  public static class TypeUtils
  {
    public static bool Is<T>(this Type type)
    {
#if PORTABLE
            return typeof (T).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
#else
      return typeof (T).IsAssignableFrom(type);
#endif
    }

    public static bool Is(this Type type, Type otherType)
    {
#if PORTABLE
            return otherType.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
#else
      return otherType.IsAssignableFrom(type);
#endif
    }

    public static bool IsDisposableType([NotNull] this Type type)
    {
      type.AssertNotNull("type != null");
#if PORTABLE
            return typeof (IDisposable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
#else
      return typeof (IDisposable).IsAssignableFrom(type);
#endif
    }

    public static bool IsDisposableType<T>()
    {
      return IsDisposableType(typeof (T));
    }

    [NotNull]
    public static Func<object> CreateDefaultConstructorFactoryOf<T>()
    {
      return CreateDefaultConstructorFactoryFor(typeof (T));
    }

    [NotNull]
    public static Func<object> CreateDefaultConstructorFactoryFor([NotNull] Type type)
    {
#if PORTABLE
            return () => Activator.CreateInstance(type);
#else
      type.IsAbstract.AssertFalse("The type '" + type + "' is abstract");
      type.IsGenericTypeDefinition.AssertFalse("The type '" + type + "' is unbound generic type");

      ConstructorInfo defaultConstuctor = type.GetConstructor(new Type[0]);
      if (defaultConstuctor == null)
        throw new ArgumentException("The type '" + type + "' has no default constructor");

      var dynamicMethod = new DynamicMethod("__Create__", typeof (object), null, type.Module);
      ILGenerator il = dynamicMethod.GetILGenerator();
      il.Emit(OpCodes.Newobj, defaultConstuctor);
      il.Emit(OpCodes.Ret);

      return (Func<object>) dynamicMethod.CreateDelegate(typeof (Func<object>));
#endif
    }

    public static IEnumerable<string> GetRequiredProperties([NotNull] this Type type)
    {
      type.AssertNotNull("type");

      var requiredAttributeType = typeof (RequiredAttribute);

#if PORTABLE
            foreach (var property in type.GetTypeInfo().DeclaredProperties)
            {
                if (property.IsDefined(requiredAttributeType))
                {
                    yield return property.Name;
                }
            }
#else
      foreach (var property in type.GetProperties())
      {
        if (Attribute.IsDefined(property, requiredAttributeType))
        {
          yield return property.Name;
        }
      }
#endif
    }
  }
}
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using JetBrains.Annotations;

namespace MicroFlow
{
    public static class TypeUtils
    {
        [NotNull]
        public static Func<object> CreateDefaultConstructorFactoryOf<T>()
        {
            return CreateDefaultConstructorFactoryFor(typeof (T));
        }

        [NotNull]
        public static Func<object> CreateDefaultConstructorFactoryFor([NotNull] Type type)
        {
            type.IsAbstract.AssertFalse("The type '" + type + "' is abstract");
            type.IsGenericTypeDefinition.AssertFalse("The type '" + type + "' is unbound generic type");

            ConstructorInfo defaultConstuctor = type.GetConstructor(new Type[0]);
            if (defaultConstuctor == null)
                throw new ArgumentException("The type '" + type + "' has not default constructor");

            var dynamicMethod = new DynamicMethod("__Create__", typeof (object), null, type.Module);
            ILGenerator il = dynamicMethod.GetILGenerator();
            il.Emit(OpCodes.Newobj, defaultConstuctor);
            il.Emit(OpCodes.Ret);

            return (Func<object>) dynamicMethod.CreateDelegate(typeof (Func<object>));
        }

        public static IEnumerable<string> GetRequiredProperties([NotNull] this Type type)
        {
            type.AssertNotNull("type");

            var requiredAttributeType = typeof (RequiredAttribute);

            foreach (var property in type.GetProperties())
            {
                if (Attribute.IsDefined(property, requiredAttributeType))
                {
                    yield return property.Name;
                }
            }
        }
    }
}
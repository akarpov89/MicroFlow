using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace MicroFlow
{
    public static class Assertion
    {
        public static T NotNull<T>(this T value) where T : class
        {
            if (null != value) return value;
            throw new AssertionException(typeof(T).FullName + " is null");
        }

        public static string NotNullOrEmpty(this string value, [NotNull] string argumentName)
        {
            if (!string.IsNullOrEmpty(value)) return value;
            throw new AssertionException(argumentName + " is null or empty");
        }

        public static string AssertNotNullOrEmpty(this string value, [NotNull] string message)
        {
            if (!string.IsNullOrEmpty(value)) return value;
            throw new AssertionException(message);
        }

        [AssertionMethod]
        public static void AssertNotNull<T>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] this T value, [NotNull] string message) where T : class
        {
            if (null != value) return;
            throw new AssertionException(message);
        }

        [AssertionMethod]
        public static void AssertIsNull<T>([AssertionCondition(AssertionConditionType.IS_NULL)] this T value, [NotNull] string message) where T : class
        {
            if (null == value) return;
            throw new AssertionException(message);
        }

        public static void AssertIsItemOf<T>(this T value, [NotNull] List<T> collection, [NotNull] string message)
        {
            if (collection.Contains(value)) return;
            throw new AssertionException(message);
        }

        public static void AssertIsNotItemOf<T>(this T value, [NotNull] List<T> collection, [NotNull] string message)
        {
            if (!collection.Contains(value)) return;
            throw new AssertionException(message);
        }

        [AssertionMethod]
        public static void AssertTrue([AssertionCondition(AssertionConditionType.IS_TRUE)] this bool condition, [NotNull] string message)
        {
            if (condition) return;
            throw new AssertionException(message);
        }

        [AssertionMethod]
        public static void AssertFalse([AssertionCondition(AssertionConditionType.IS_FALSE)] this bool condition, [NotNull] string message)
        {
            if (!condition) return;
            throw new AssertionException(message);
        }

        [Serializable]
        public class AssertionException : Exception
        {
            public AssertionException(string message)
              : base(message)
            {
            }

            protected AssertionException(SerializationInfo info, StreamingContext context)
              : base(info, context)
            {
            }
        }
    }
}
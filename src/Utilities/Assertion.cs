using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace MicroFlow
{
    internal static class Assertion
    {
        [DebuggerStepThrough]
        public static T NotNull<T>(this T value) where T : class
        {
            if (null != value) return value;
            throw new AssertionException(typeof(T).FullName + " is null");
        }

        [DebuggerStepThrough]
        public static string NotNullOrEmpty(this string value, [NotNull] string argumentName)
        {
            if (!string.IsNullOrEmpty(value)) return value;
            throw new AssertionException(argumentName + " is null or empty");
        }

        [DebuggerStepThrough]
        public static string AssertNotNullOrEmpty(this string value, [NotNull] string message)
        {
            if (!string.IsNullOrEmpty(value)) return value;
            throw new AssertionException(message);
        }

        [DebuggerStepThrough, AssertionMethod]
        public static void AssertNotNull<T>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] this T value, [NotNull] string message) where T : class
        {
            if (null != value) return;
            throw new AssertionException(message);
        }

        [DebuggerStepThrough, AssertionMethod]
        public static void AssertIsNull<T>([AssertionCondition(AssertionConditionType.IS_NULL)] this T value, [NotNull] string message) where T : class
        {
            if (null == value) return;
            throw new AssertionException(message);
        }

        [DebuggerStepThrough]
        public static void AssertIsItemOf<T>(this T value, [NotNull] List<T> collection, [NotNull] string message)
        {
            if (collection.Contains(value)) return;
            throw new AssertionException(message);
        }

        [DebuggerStepThrough]
        public static void AssertIsNotItemOf<T>(this T value, [NotNull] List<T> collection, [NotNull] string message)
        {
            if (!collection.Contains(value)) return;
            throw new AssertionException(message);
        }

        [DebuggerStepThrough, AssertionMethod]
        public static void AssertTrue([AssertionCondition(AssertionConditionType.IS_TRUE)] this bool condition, [NotNull] string message)
        {
            if (condition) return;
            throw new AssertionException(message);
        }

        [DebuggerStepThrough, AssertionMethod]
        public static void AssertFalse([AssertionCondition(AssertionConditionType.IS_FALSE)] this bool condition, [NotNull] string message)
        {
            if (!condition) return;
            throw new AssertionException(message);
        }

#if PORTABLE
        [DataContract]
#else
        [Serializable]
#endif
        public class AssertionException : Exception
        {
            public AssertionException(string message)
              : base(message)
            {
            }
#if !PORTABLE
            protected AssertionException(SerializationInfo info, StreamingContext context)
              : base(info, context)
            {
            }
#endif
        }
    }
}
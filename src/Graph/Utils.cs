using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace MicroFlow.Graph
{
    internal static class Utils
    {
        [NotNull, ItemNotNull]
        public static IEnumerable<XAttribute> ToAttributes([CanBeNull] this Dictionary<string, object> properties)
        {
            return properties?.Select(p => new XAttribute(p.Key, p.Value)) ?? Enumerable.Empty<XAttribute>();
        }

        public static void SetOptionalAttribute(
            [NotNull] this XElement element, [NotNull] string name, [CanBeNull] object value)
        {
            if (value != null)
            {
                element.SetAttributeValue(name, value);
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MicroFlow.Graph
{
    internal static class Utils
    {
        public static IEnumerable<XAttribute> ToAttributes(this Dictionary<string, object> properties)
        {
            return properties?.Select(p => new XAttribute(p.Key, p.Value)) ?? Enumerable.Empty<XAttribute>();
        }

        public static void SetOptionalAttribute(this XElement element, string name, object value)
        {
            if (value != null)
            {
                element.SetAttributeValue(name, value);
            }
        }
    }
}
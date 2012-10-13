using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using Configuration.Xml.Protected;
using System.Collections.Concurrent;

namespace Configuration.Xml
{
	public static class XmlDeserializer<T> where T : class
	{
		private static WeakReference _weakCache = new WeakReference(null);

		public static T Deserialize(XElement element)
		{
			if (element == null)
				return null;

			using (XmlReader xr = element.CreateReader())
				return (T)GetSerializer(element.Name).Deserialize(xr);
		}

		public static XmlSerializer GetSerializer(XName name)
		{
			ConcurrentDictionary<XName, XmlSerializer> cache = null;
			lock (_weakCache)
			{
				var target = _weakCache.Target;
				if(target == null)
				{
					cache = new ConcurrentDictionary<XName, XmlSerializer>();
					_weakCache.Target = cache;
				}
				else
					cache = target as ConcurrentDictionary<XName, XmlSerializer>;
			}

			return cache.GetOrAdd(name, CreateSerializer);
		}

		private static XmlSerializer CreateSerializer(XName name)
		{
			var rootAttr = new XmlRootAttribute();
			rootAttr.ElementName = name.LocalName;
			rootAttr.Namespace = name.NamespaceName;

			return new XmlSerializer(typeof(T), rootAttr);
		}
	}
}


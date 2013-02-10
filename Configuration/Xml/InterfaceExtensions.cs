using System;
using System.Linq;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Configuration.GenericView;


namespace Configuration.Xml
{
	public static class InterfaceExtensions
	{
		public static ICfgNode CreateView(this IPlainConverter converter, XDocument doc)
		{
			return new XmlViewNode(converter, doc.Root);
		}

		public static ICfgNode CreateView(this IPlainConverter converter, XElement el)
		{
			return new XmlViewNode(converter, el);
		}
	}
}


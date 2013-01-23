using System;
using System.Linq;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml.Linq;


namespace Configuration.GenericView
{
	public static class InterfaceExtensions
	{
		public static ICfgNode CreateView(this IXmlViewConverter converter, XDocument doc)
		{
			return new XmlViewNode(converter, doc.Root);
		}

		public static ICfgNode CreateView(this IXmlViewConverter converter, XElement el)
		{
			return new XmlViewNode(converter, el);
		}
	}
}


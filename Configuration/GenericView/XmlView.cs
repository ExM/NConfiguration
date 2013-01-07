using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace Configuration.GenericView
{
	public static class XmlView
	{
		public static ICfgNode Create(XDocument doc)
		{
			return new XmlViewNode(new XmlViewSettings(), doc.Root);
		}
	}
}


using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Configuration
{
	/// <summary>
	/// settings loaded from a XML document
	/// </summary>
	public abstract class XmlSettings : IXmlSettings
	{
		private XDocument _doc;

		protected XmlSettings(XDocument doc)
		{
			if(doc == null)
				throw new ArgumentNullException("doc");
			if(doc.Root == null)
				throw new FormatException("root element not found");
			_doc = doc;
		}

		public XElement GetSection(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			
			return _doc.Root.Element(XNamespace.None + name);
		}
	}
}


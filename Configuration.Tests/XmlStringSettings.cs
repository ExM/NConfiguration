using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Configuration
{
	public class XmlStringSettings : XmlSettings
	{
		public XmlStringSettings(string text)
			: base(XDocument.Parse(text))
		{
		}
	}
}


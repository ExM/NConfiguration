using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using Configuration.Xml;

namespace Configuration
{
	public class XmlStringSettings : XmlSettings
	{
		public XmlStringSettings(string text)
		{
			Root = XDocument.Parse(text).Root;
		}
	}
}


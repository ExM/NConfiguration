using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Configuration;

namespace Configuration
{
	public class PlainXmlSection : ConfigurationSection
	{
		public XDocument PlainXml { get; set; }

		protected override void DeserializeSection(XmlReader reader)
		{
			PlainXml = XDocument.Load(reader);
		}
	}
}

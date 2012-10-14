using System;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Configuration.Xml.ConfigSections
{
	[XmlRoot("configProtectedData")]
	public class ConfigProtectedData
	{
		[XmlAnyElement("providers")]
		public XmlElement Providers { get; set; }
	}
}


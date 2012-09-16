using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Configuration
{
	[XmlRoot("MyXmlCfg")]
	public class MyXmlConfig
	{
		[XmlAttribute]
		public string AttrField = null;

		[XmlElement]
		public string ElemField = null;
	}
}


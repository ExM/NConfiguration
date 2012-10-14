using System;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Configuration.Xml.ConfigSections
{
	[XmlRoot("Include")]
	public class IncludeConfig
	{
		[XmlAttribute("FinalSearch")]
		public bool FinalSearch { get; set;}
	}
}


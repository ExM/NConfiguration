using System;
using System.Xml.Serialization;


namespace Configuration.ConfigSections
{
	public class IncludeFileConfig
	{
		[XmlAttribute("Path")]
		public string Path { get; set;}
		
		[XmlAttribute("Search")]
		public SearchMode Search { get; set;}
		
		[XmlAttribute("Include")]
		public IncludeMode Include { get; set;}
		
		[XmlAttribute("Required")]
		public bool Required { get; set;}
	}
}


using System;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace NConfiguration.ExampleTypes
{
	public class CombinableConfig
	{
		[XmlAttribute("Field1")]
		public string Field1 { get; set; }
	}
}


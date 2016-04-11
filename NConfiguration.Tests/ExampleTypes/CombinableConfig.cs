using System;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NConfiguration.ExampleTypes
{
	public class CombinableConfig
	{
		[DataMember(Name = "Field1")]
		public string Field1 { get; set; }
	}
}


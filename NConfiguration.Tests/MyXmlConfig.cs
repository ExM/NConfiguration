using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NConfiguration
{
	[DataContract(Name = "MyXmlCfg")]
	public class MyXmlConfig
	{
		[DataMember]
		public string AttrField = "default";

		[DataMember]
		public string ElemField = null;
	}
}


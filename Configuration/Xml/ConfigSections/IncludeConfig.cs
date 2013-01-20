using System;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Configuration.Xml.ConfigSections
{
	[DataContract(Name = "Include")]
	public class IncludeConfig
	{
		[DataMember(Name = "FinalSearch",  IsRequired = false)]
		public bool FinalSearch { get; set;}
	}
}


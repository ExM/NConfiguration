using System;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Configuration.GenericView;

namespace Configuration.Xml.ConfigSections
{
	[DataContract(Name = "configProtectedData")]
	public class ConfigProtectedData
	{
		[DataMember(Name = "providers")]
		public ICfgNode Providers { get; set; }
	}
}


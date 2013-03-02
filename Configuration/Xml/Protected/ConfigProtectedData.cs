using System.Runtime.Serialization;
using Configuration.GenericView;

namespace Configuration.Xml.Protected
{
	[DataContract(Name = "configProtectedData")]
	public class ConfigProtectedData
	{
		[DataMember(Name = "providers")]
		public ICfgNode Providers { get; set; }
	}
}


using System.Runtime.Serialization;

namespace Demo
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


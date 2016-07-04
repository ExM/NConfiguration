using System.Runtime.Serialization;

namespace NConfiguration.Combination
{
	public class TestAttrClass
	{
		[IgnoreDataMember]
		public string FString1;
		
		public string PString { get; set;}
	}
}


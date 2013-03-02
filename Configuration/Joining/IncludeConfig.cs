using System.Runtime.Serialization;

namespace Configuration.Joining
{
	[DataContract(Name = "Include")]
	public class IncludeConfig
	{
		[DataMember(Name = "FinalSearch",  IsRequired = false)]
		public bool FinalSearch { get; set;}
	}
}


using System.Runtime.Serialization;

namespace NConfiguration.Joining
{
	[DataContract(Name = "Include")]
	public class IncludeConfig
	{
		[DataMember(Name = "FinalSearch",  IsRequired = false)]
		public bool FinalSearch { get; set;}
	}
}


using System;
using System.Xml.Serialization;
using Configuration.Joining;
using System.Runtime.Serialization;


namespace Configuration.Xml.ConfigSections
{
	public class IncludeFileConfig
	{
		[DataMember(Name = "Path", IsRequired = true)]
		public string Path { get; set;}

		[DataMember(Name = "Search", IsRequired = false)]
		public SearchMode Search { get; set;}

		[DataMember(Name = "Include", IsRequired = false)]
		public IncludeMode Include { get; set;}

		[DataMember(Name = "Required", IsRequired = false)]
		public bool Required { get; set;}
	}
}


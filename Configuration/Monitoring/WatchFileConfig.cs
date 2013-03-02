using System;
using System.Runtime.Serialization;

namespace Configuration.Monitoring
{
	[DataContract(Name="WatchFile")]
	public class WatchFileConfig
	{
		[DataMember(Name = "Mode", IsRequired = false)]
		public WatchMode Mode { get; set; }

		[DataMember(Name = "Delay", IsRequired = false)]
		public TimeSpan? Delay { get; set;}
	}
}


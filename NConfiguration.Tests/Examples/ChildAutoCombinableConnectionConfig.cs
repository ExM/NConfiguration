using System;
using System.Runtime.Serialization;

namespace NConfiguration.Examples
{
	[DataContract(Name = "CustomConnection")]
	public class ChildAutoCombinableConnectionConfig : AutoCombinableConnectionConfig
	{
		[DataMember(Name = "Timeout")]
		public TimeSpan? Timeout { get; set; }

		[IgnoreDataMember]
		public override string ConnectionString
		{
			get
			{
				if(Timeout == null)
					return base.ConnectionString;

				if(Timeout.Value.TotalSeconds < 1)
					throw new ArgumentOutOfRangeException("Timeout");

				var result = base.ConnectionString;

				if (result != "")
					result += ";";

				result += "Connection Timeout=" + ((long)Timeout.Value.TotalSeconds).ToString();

				return result;
			}
		}
	}
}


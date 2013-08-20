using System;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace NConfiguration.Examples
{
	[XmlRoot("CustomConnection")]
	public class ChildAutoCombinableConnectionConfig : AutoCombinableConnectionConfig
	{
		[XmlAttribute("Timeout")]
		public TimeSpan? Timeout { get; set; }

		[XmlIgnore]
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


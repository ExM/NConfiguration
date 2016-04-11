using System;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NConfiguration.Examples
{
	[DataContract(Name = "Connection")]
	public class AutoCombinableConnectionConfig
	{
		[DataMember(Name = "Server")]
		public string Server { get; set; }

		[DataMember(Name = "Database")]
		public string Database { get; set; }

		[DataMember(Name = "User")]
		public string User { get; set; }

		[DataMember(Name = "Password")]
		public string Password { get; set; }

		[DataMember(Name = "Additional")]
		public string Additional { get; set; }

		[IgnoreDataMember]
		public virtual string ConnectionString
		{
			get
			{
				var sb = new StringBuilder();
				Set(sb, "Server", Server);
				Set(sb, "Database", Database);
				Set(sb, "User ID", User);
				Set(sb, "Password", Password);

				if (!string.IsNullOrWhiteSpace(Additional))
					sb.Append(Additional[0] == ';' ? Additional.Substring(1) : Additional);

				return sb.ToString();
			}
		}

		private static void Set(StringBuilder sb, string name, string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return;
			sb.AppendFormat("{0}={1};", name, value);
		}
	}
}


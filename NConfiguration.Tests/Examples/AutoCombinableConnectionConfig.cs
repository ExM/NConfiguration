using System;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace NConfiguration.Examples
{
	[XmlRoot("Connection")]
	public class AutoCombinableConnectionConfig
	{
		[XmlAttribute("Server")]
		public string Server { get; set; }

		[XmlAttribute("Database")]
		public string Database { get; set; }

		[XmlAttribute("User")]
		public string User { get; set; }

		[XmlAttribute("Password")]
		public string Password { get; set; }

		[XmlAttribute("Additional")]
		public string Additional { get; set; }

		[XmlIgnore]
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


using System.Text;
using NConfiguration.Combination;
using System.Runtime.Serialization;

namespace NConfiguration.Examples
{
	public class ConnectionConfig : ICombinable, ICombinable<ConnectionConfig>
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
		public string ConnectionString
		{
			get
			{
				var sb = new StringBuilder();
				set(sb, "Server", Server);
				set(sb, "Database", Database);
				set(sb, "User ID", User);
				set(sb, "Password", Password);

				if (!string.IsNullOrWhiteSpace(Additional))
					sb.Append(Additional[0] == ';' ? Additional.Substring(1) : Additional);

				return sb.ToString();
			}
		}

		private static void set(StringBuilder sb, string name, string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return;
			sb.AppendFormat("{0}={1};", name, value);
		}

		public void Combine(ICombiner combiner, ConnectionConfig other)
		{
			if (other == null)
				return;

			Server = other.Server ?? Server;
			Database = other.Database ?? Database;
			User = other.User ?? User;
			Password = other.Password ?? Password;
			Additional = other.Additional ?? Additional;
		}

		public virtual void Combine(ICombiner combiner, object other)
		{
			Combine(combiner, other as ConnectionConfig);
		}
	}
}


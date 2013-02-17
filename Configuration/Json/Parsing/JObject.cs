using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configuration.Json.Parsing
{
	public sealed class JObject : JValue
	{
		public List<KeyValuePair<string, JValue>> Properties { get; private set; }

		public JObject()
		{
			Properties = new List<KeyValuePair<string, JValue>>();
		}

		public override TokenType Type
		{
			get
			{
				return TokenType.Object;
			}
		}
	}
}

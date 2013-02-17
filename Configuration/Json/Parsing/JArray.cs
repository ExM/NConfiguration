using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configuration.Json.Parsing
{
	public class JArray: JValue
	{
		public List<JValue> Items { get; private set; }

		public JArray()
		{
			Items = new List<JValue>();
		}

		public override TokenType Type
		{
			get
			{
				return TokenType.Array;
			}
		}
	}
}

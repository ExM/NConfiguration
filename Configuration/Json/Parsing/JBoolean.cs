using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configuration.Json.Parsing
{
	public class JBoolean: JValue
	{
		public string Value { get; private set; }

		public JBoolean(string text)
		{
			Value = text;
		}

		public override TokenType Type
		{
			get
			{
				return TokenType.Boolean;
			}
		}
	}
}

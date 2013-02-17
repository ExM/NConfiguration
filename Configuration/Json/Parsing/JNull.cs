using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configuration.Json.Parsing
{
	public class JNull: JValue
	{
		public static readonly JNull Instance = new JNull();

		public JNull()
		{
		}

		public override TokenType Type
		{
			get
			{
				return TokenType.Null;
			}
		}
	}
}

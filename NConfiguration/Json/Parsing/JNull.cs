using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConfiguration.Json.Parsing
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

		public override string ToString()
		{
			return "null";
		}
	}
}

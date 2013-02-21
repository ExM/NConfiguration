using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configuration.Json.Parsing
{
	public abstract class JValue
	{
		public abstract TokenType Type { get; }

		public static JValue Parse(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				throw new ArgumentNullException("text");

			var chars = new CharEnumerator(text);
			var result = Tools.ParseValue(chars, true);

			if(chars.MoveNext())
				throw new FormatException(string.Format("unexpected symbol '{0}' in the reading of end", chars.Current));

			return result;
		}
	}
}

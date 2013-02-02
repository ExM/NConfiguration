using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configuration.Ini.Parsing
{
	public class Token
	{
		public TokenType Type { get; private set; }

		private StringBuilder _text = new StringBuilder();

		public Token(TokenType type)
		{
			Type = type;
		}

		public void Append(char ch)
		{
			_text.Append(ch);
		}

		public string Text
		{
			get
			{
				return _text.ToString();
			}
		}
	}
}

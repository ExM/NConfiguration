using System;
using System.Linq;
using System.Runtime.Serialization;
using Configuration.GenericView;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Configuration.Json.Parsing
{
	public static class Tools
	{
		public static bool MoveTo(this ICharEnumerator chars, Char end)
		{
			while (chars.MoveNext())
			{
				var cur = chars.Current;
				if (Char.IsWhiteSpace(cur))
					continue;
				if(cur == end)
					return true;
				throw new FormatException(string.Format("unexpected symbol '{0}'", cur));
			}

			return false;
		}

		public static bool MoveToNoWhite(this ICharEnumerator chars)
		{
			while (chars.MoveNext())
			{
				if (!Char.IsWhiteSpace(chars.Current))
					return true;
			}

			return false;
		}

		public static bool MoveTo(this ICharEnumerator chars, params Char[] ends)
		{
			int N = ends.Length;

			while (chars.MoveNext())
			{
				var cur = chars.Current;
				if (Char.IsWhiteSpace(cur))
					continue;

				for (int i = 0; i < N; i++)
					if (cur == ends[i])
						return true;

				throw new FormatException(string.Format("unexpected symbol '{0}'", cur));
			}

			return false;
		}

		internal static JString ParseString(ICharEnumerator chars)
		{
			StringBuilder text = new StringBuilder();

			while (true)
			{
				if (!chars.MoveNext())
					throw new FormatException("unexpected end in the reading of string");

				switch (chars.Current)
				{
					case '\"':
						return new JString(text.ToString());
					case '\\':
						text.Append(ParseEscapeSymbol(chars));
						break;
					case '\b':
					case '\f':
					case '\r':
					case '\n':
					case '\t':
						throw new FormatException("unexpected control character in the reading of string");
					default:
						text.Append(chars.Current);
						break;
				}
			}
		}

		private static char ParseEscapeSymbol(ICharEnumerator chars)
		{
			if (!chars.MoveNext())
				throw new FormatException("unexpected end in the reading of string");

			switch (chars.Current)
			{
				case '\"':
					return '\"';
				case '\\':
					return '\\';
				case '/':
					return '/';
				case 'b':
					return '\b';
				case 'f':
					return '\f';
				case 'r':
					return '\r';
				case 'n':
					return '\n';
				case 't':
					return '\t';
				case 'u':
					return ReadUnicodeSymbol(chars);
				default:
					throw new FormatException(string.Format("unexpected escape symbol '{0}'", chars.Current));
			}
		}

		private static char ReadUnicodeSymbol(ICharEnumerator chars)
		{
			StringBuilder text = new StringBuilder(4);

			for (int i = 0; i < 4; i++)
			{
				if (!chars.MoveNext())
					throw new FormatException("unexpected end in the reading of string");
				if(!chars.Current.IsHexDigit())
					throw new FormatException(string.Format("unexpected symbol '{0}' in hex digit in unicode symbol", chars.Current));
				text.Append(chars.Current);
			}
			
			var num = UInt16.Parse(text.ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			return Convert.ToChar(num);
		}

		/// <summary>
		/// Is a character 0-9 a-f A-F ?
		/// </summary>
		public static bool IsHexDigit(this char c)
		{
			if ('0' <= c && c <= '9') return true;
			if ('a' <= c && c <= 'f') return true;
			if ('A' <= c && c <= 'F') return true;
			return false;
		}

		internal static JArray ParseArray(ICharEnumerator chars)
		{
			var result = new JArray();

			if (!chars.MoveToNoWhite())
				throw new FormatException("unexpected end in the reading of array");

			if (chars.Current == ']')
				return result;

			result.Items.Add(ParseValue(chars, false));

			while (true)
			{
				if (!chars.MoveToNoWhite())
					throw new FormatException("unexpected end in the reading of array");

				if (chars.Current == ']')
					return result;

				if (chars.Current != ',')
					throw new FormatException(string.Format("unexpected symbol '{0}' in the reading of array", chars.Current));

				if (!chars.MoveToNoWhite())
					throw new FormatException("unexpected end in the reading of array");

				result.Items.Add(ParseValue(chars, false));
			}
		}

		internal static JBoolean ParseBoolean(ICharEnumerator chars)
		{
			if (chars.Current == 't')
			{
				chars.ExpectedRead('r', "boolean");
				chars.ExpectedRead('u', "boolean");
				chars.ExpectedRead('e', "boolean");

				return new JBoolean("true");
			}
			else
			{
				chars.ExpectedRead('a', "boolean");
				chars.ExpectedRead('l', "boolean");
				chars.ExpectedRead('s', "boolean");
				chars.ExpectedRead('e', "boolean");

				return new JBoolean("false");
			}
		}

		internal static JNull ParseNull(ICharEnumerator chars)
		{
			chars.ExpectedRead('u', "null");
			chars.ExpectedRead('l', "null");
			chars.ExpectedRead('l', "null");

			return JNull.Instance;
		}

		private static void ExpectedRead(this ICharEnumerator chars, char ch, string tokenName)
		{
			if (!chars.MoveNext())
				throw new FormatException(string.Format("unexpected end in the reading of {0}", tokenName));
			if (chars.Current != ch)
				throw new FormatException(string.Format("unexpected symbol '{0}' in the reading of {1}", chars.Current, tokenName));
		}

		internal static JNumber ParseNumber(ICharEnumerator chars)
		{
			StringBuilder text = new StringBuilder();

			if (chars.Current == '-')
			{
				text.Append('-');
				if (!chars.MoveNext())
					throw new FormatException("unexpected end in the reading of number");
			}

			//TODO: fix parsing

			while (true)
			{
				if (('0' <= chars.Current && chars.Current <= '9') ||
					chars.Current == 'E' ||
					chars.Current == 'e' ||
					chars.Current == '.' ||
					chars.Current == '-' ||
					chars.Current == '+')
				{
					text.Append(chars.Current);
					if (!chars.MoveNext())
						throw new FormatException("unexpected end in the reading of number");
				}
				else
				{
					chars.MovePrev();
					return new JNumber(text.ToString());
				}
			}
		}

		internal static JObject ParseObject(ICharEnumerator chars)
		{
			var result = new JObject();

			if (!chars.MoveTo('\"', '}'))
				throw new FormatException("unexpected end in the reading of object");

			if (chars.Current == '}')
				return result;

			while (true)
			{
				var propName = ParseString(chars);

				if (!chars.MoveTo(':'))
					throw new FormatException("unexpected end in the reading of object");

				var propValue = ParseValue(chars, true);

				result.Properties.Add(new KeyValuePair<string, JValue>(propName.Value, propValue));

				if (!chars.MoveTo(',', '}'))
					throw new FormatException("unexpected end in the reading of object");

				if (chars.Current == '}')
					return result;

				if (!chars.MoveTo('\"'))
					throw new FormatException("unexpected end in the reading of object");
			}
		}

		public static JValue ParseValue(ICharEnumerator chars, bool move)
		{
			if(move && !chars.MoveNext())
				throw new FormatException("unexpected end in the reading of value");

			while (true)
			{
				if (Char.IsWhiteSpace(chars.Current))
				{
					if (!chars.MoveNext())
						throw new FormatException("unexpected end in the reading of value");
					continue;
				}

				switch (chars.Current)
				{
					case 't':
					case 'f':
						return ParseBoolean(chars);

					case 'n':
						return ParseNull(chars);

					case '{':
						return ParseObject(chars);

					case '\"':
						return ParseString(chars);

					case '[':
						return ParseArray(chars);

					case '-':
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						return ParseNumber(chars);

					default:
						throw new FormatException(string.Format("unexpected symbol '{0}' in the reading of value", chars.Current));
				}
			}
		}
	}
}


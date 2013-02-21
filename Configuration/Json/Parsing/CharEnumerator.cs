using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Configuration.Json.Parsing
{
	internal class CharEnumerator
	{
		private string _text;
		private int _current = -1;

		public CharEnumerator(string text)
		{
			_text = text;
		}

		public char Current
		{
			get
			{
				return _text[_current];
			}
		}

		public bool MoveNext()
		{
			if (_current == -1)
			{
				_current = 0;
				return true;
			}
			
			if (_current == _text.Length - 1)
				return false;

			_current++;
			return true;
		}

		private static Regex _number = new Regex(@"-?(?:0|[1-9]\d*)(?:\.\d+)?(?:[eE][+-]?\d+)?", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		internal string ReadNumber()
		{
			var m = _number.Match(_text, _current);
			if (!m.Success)
				throw new FormatException("invalid format of number");

			var result = m.Value;

			_current += result.Length - 1;
			return result;
		}

		internal void ExpectedRead(char ch, string tokenName)
		{
			if (!MoveNext())
				throw new FormatException(string.Format("unexpected end in the reading of {0}", tokenName));
			if (Current != ch)
				throw new FormatException(string.Format("unexpected symbol '{0}' in the reading of {1}", Current, tokenName));
		}

		internal bool MoveTo(Char end)
		{
			while (MoveNext())
			{
				var cur = Current;
				if (Char.IsWhiteSpace(cur))
					continue;
				if (cur == end)
					return true;
				throw new FormatException(string.Format("unexpected symbol '{0}'", cur));
			}

			return false;
		}

		internal bool MoveToNoWhite()
		{
			while (MoveNext())
			{
				if (!Char.IsWhiteSpace(Current))
					return true;
			}

			return false;
		}

		internal bool MoveTo(params Char[] ends)
		{
			int N = ends.Length;

			while (MoveNext())
			{
				var cur = Current;
				if (Char.IsWhiteSpace(cur))
					continue;

				for (int i = 0; i < N; i++)
					if (cur == ends[i])
						return true;

				throw new FormatException(string.Format("unexpected symbol '{0}'", cur));
			}

			return false;
		}
	}
}

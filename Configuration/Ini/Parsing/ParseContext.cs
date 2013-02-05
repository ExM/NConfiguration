using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Configuration.Ini.Parsing
{
	public class ParseContext
	{
		private char BeginComment = ';';
		private char BeginSection = '[';
		private char EndSection = ']';
		private char KeyValueSeparator = '=';
		private char NewLine = '\n';
		private char CarriageReturn = '\r';
		private char TextQuote = '\"';

		private LinkedList<Section> _sections = new LinkedList<Section>();

		private StringBuilder _tokenValue = new StringBuilder();
		private string _curentKey = null;
		private ParseState _state = ParseState.BeginLine;

		public ParseContext()
		{
			_sections.AddLast(new Section(string.Empty));
		}

		public void ParseSource(IEnumerable<char> chars)
		{
			if (_state != ParseState.BeginLine)
				throw new NotImplementedException("unexpected state: " + _state.ToString());

			foreach (char ch in chars)
				_state = AppendChar(ch);

			EndChars();
		}

		private void EndChars()
		{
			switch (_state)
			{
				case ParseState.BeginLine:
				case ParseState.EmptyLine:
				case ParseState.Comment:
				case ParseState.SectionEnd:
				case ParseState.EndQuotedValue:
					return;

				case ParseState.SectionName:
					throw new FormatException("unexpected end in section name");

				case ParseState.KeyName:
				case ParseState.EndKeyName:
					throw new FormatException("unexpected end line in key name");

				case ParseState.BeginValue:
				case ParseState.SimpleValue:
				case ParseState.EscValue:
					EndValue();
					return;

				case ParseState.QuotedValue:
					throw new FormatException("unexpected end line in quoted value");

				default:
					throw new NotImplementedException("unexpected state: " + _state.ToString());
			}
		}

		private ParseState AppendChar(char ch)
		{
			//Console.WriteLine("{0} <{1}>", _state, ch);
			switch (_state)
			{
				case ParseState.BeginLine:
					return ProcessBeginLine(ch);

				case ParseState.EmptyLine:
					return ProcessEmptyLine(ch);

				case ParseState.Comment:
					return ProcessComment(ch);

				case ParseState.SectionName:
					return ProcessSectionName(ch);

				case ParseState.SectionEnd:
					return ProcessSectionEnd(ch);

				case ParseState.KeyName:
					return ProcessKeyName(ch);

				case ParseState.EndKeyName:
					return ProcessEndKeyName(ch);

				case ParseState.BeginValue:
					return ProcessBeginValue(ch);

				case ParseState.SimpleValue:
					return ProcessSimpleValue(ch);

				case ParseState.QuotedValue:
					return ProcessQuotedValue(ch);

				case ParseState.EscValue:
					return ProcessEscValue(ch);

				case ParseState.EndQuotedValue:
					return ProcessEndQuotedValue(ch);

				default:
					throw new NotImplementedException("unexpected state: " + _state.ToString());
			}
		}

		private ParseState ProcessEndQuotedValue(char ch)
		{
			if(ch == BeginComment)
				return ParseState.Comment;

			if(ch == NewLine || ch == CarriageReturn)
				return ParseState.BeginLine;

			if(Char.IsWhiteSpace(ch))
				return ParseState.EndQuotedValue;

			throw new FormatException(string.Format("unexpected char '{0}' after '{1}'", ch, TextQuote));
		}

		private ParseState ProcessEscValue(char ch)
		{
			if(ch == TextQuote)
			{
				_tokenValue.Append(ch);
				return ParseState.QuotedValue;
			}

			if(ch == BeginComment)
			{
				EndValue();
				return ParseState.Comment;
			}

			if(ch == NewLine || ch == CarriageReturn)
			{
				EndValue();
				return ParseState.BeginLine;
			}

			if(Char.IsWhiteSpace(ch))
			{
				EndValue();
				return ParseState.EndQuotedValue;
			}

			throw new FormatException(string.Format("unexpected char '{0}' after '{1}'", ch, TextQuote));
		}

		private ParseState ProcessQuotedValue(char ch)
		{
			if(ch == TextQuote)
				return ParseState.EscValue;

			// *
			_tokenValue.Append(ch);
			return ParseState.QuotedValue;
		}

		private ParseState ProcessSimpleValue(char ch)
		{
			if (ch == BeginComment)
			{
				EndValue();
				return ParseState.Comment;
			}

			if (ch == NewLine || ch == CarriageReturn)
			{
				EndValue();
				return ParseState.BeginLine;
			}

			// *
			_tokenValue.Append(ch);
			return ParseState.SimpleValue;
		}

		private ParseState ProcessBeginValue(char ch)
		{
			if (Char.IsWhiteSpace(ch))
				return ParseState.BeginValue;

			if (ch == NewLine || ch == CarriageReturn)
			{
				EndValue();
				return ParseState.BeginLine;
			}

			if (ch == TextQuote)
				return ParseState.QuotedValue;

			// *
			_tokenValue.Append(ch);
			return ParseState.SimpleValue;
		}

		private void EndValue()
		{
			var value = _tokenValue.ToString();
			_tokenValue.Clear();

			_sections.Last.Value.Pairs.Add(new KeyValuePair<string,string>(_curentKey, value));
		}

		private ParseState ProcessEndKeyName(char ch)
		{
			if (ch == NewLine || ch == CarriageReturn)
				throw new FormatException("unexpected end line in key name");

			if (Char.IsWhiteSpace(ch))
				return ParseState.EndKeyName;

			if (ch == KeyValueSeparator)
				return ParseState.BeginValue;

			throw new FormatException(string.Format("unexpected char '{0}' before '{1}'", ch, KeyValueSeparator));
		}

		private ParseState ProcessKeyName(char ch)
		{
			if (ch == NewLine || ch == CarriageReturn)
				throw new FormatException("unexpected end line in key name");

			if (Char.IsWhiteSpace(ch))
			{
				KeyNameEnd();
				return ParseState.EndKeyName;
			}

			if (ch == KeyValueSeparator)
			{
				KeyNameEnd();
				return ParseState.BeginValue;
			}

			// *
			_tokenValue.Append(ch);
			return ParseState.KeyName;
		}

		private void KeyNameEnd()
		{
			_curentKey = _tokenValue.ToString();
			_tokenValue.Clear();
		}

		private ParseState ProcessSectionEnd(char ch)
		{
			if (ch == NewLine || ch == CarriageReturn)
				return ParseState.BeginLine;

			if (ch == BeginComment)
				return ParseState.Comment;

			if (Char.IsWhiteSpace(ch))
				return ParseState.SectionEnd;

			throw new FormatException(string.Format("non white char '{0}' after section name", ch));
		}

		private ParseState ProcessSectionName(char ch)
		{
			if (ch == NewLine || ch == CarriageReturn)
				throw new FormatException("unexpected end line in section name");


			if (ch == EndSection)
			{
				EndSectionName();
				return ParseState.SectionEnd;
			}

			_tokenValue.Append(ch);
			return ParseState.SectionName;
		}

		private void EndSectionName()
		{
			var sectionName = _tokenValue.ToString();
			_sections.AddLast(new Section(sectionName));
			_tokenValue.Clear();
		}

		private ParseState ProcessComment(char ch)
		{
			if (ch == NewLine)
				return ParseState.BeginLine;

			return ParseState.Comment;
		}

		private ParseState ProcessEmptyLine(char ch)
		{
			if (ch == NewLine)
				return ParseState.BeginLine;

			if (Char.IsWhiteSpace(ch))
				return ParseState.EmptyLine;

			throw new FormatException(string.Format("non white char '{0}' in empty line", ch));
		}

		private ParseState ProcessBeginLine(char ch)
		{
			if (ch == NewLine)
				return ParseState.BeginLine;

			if (ch == CarriageReturn)
				return ParseState.BeginLine;

			if (Char.IsWhiteSpace(ch))
				return ParseState.EmptyLine;

			if (ch == BeginComment)
				return ParseState.Comment;

			if (ch == BeginSection)
				return ParseState.SectionName;

			// *
			_tokenValue.Append(ch);
			return ParseState.KeyName;
		}

		public IEnumerable<Section> Sections
		{
			get
			{
				if (_sections.First.Value.Name == string.Empty &&
					_sections.First.Value.Pairs.Count == 0)
					return _sections.Skip(1);

				return _sections;
			}
		}
	}
}

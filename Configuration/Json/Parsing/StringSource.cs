using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configuration.Json.Parsing
{
	public class StringSource: ICharEnumerator
	{
		private string _text;
		private int _current = -1;

		public StringSource(string text)
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

		public void MovePrev()
		{
			if (_current == -1)
				return;

			_current--;
		}
	}
}

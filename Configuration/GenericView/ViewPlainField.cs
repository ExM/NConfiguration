using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Configuration.GenericView
{
	public class ViewPlainField: ICfgNode
	{
		private string _text;
		private IStringConverter _converter;

		public ViewPlainField(IStringConverter converter, string text)
		{
			_converter = converter;
			_text = text;
		}

		public ICfgNode GetChild(string name)
		{
			return null;
		}

		public IEnumerable<ICfgNode> GetCollection(string name)
		{
			yield break;
		}

		public T As<T>()
		{
			return _converter.Convert<T>(_text);
		}

		public IEnumerable<KeyValuePair<string, ICfgNode>> GetNodes()
		{
			yield break;
		}
	}
}


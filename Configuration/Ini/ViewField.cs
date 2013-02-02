using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Configuration.GenericView;

namespace Configuration.Ini
{
	internal class ViewField: ICfgNode
	{
		private string _text;
		private IXmlViewConverter _converter;

		public ViewField(IXmlViewConverter converter, string text)
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


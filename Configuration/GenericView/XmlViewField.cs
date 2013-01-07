using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Configuration.GenericView
{
	internal class XmlViewField: ICfgNode
	{
		private string _text;
		private XmlViewSettings _settings;

		public XmlViewField(XmlViewSettings settings, string text)
		{
			_settings = settings;
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
			return _settings.Convert<T>(_text);
		}

		public IEnumerable<KeyValuePair<string, ICfgNode>> GetNodes()
		{
			yield break;
		}
	}
}


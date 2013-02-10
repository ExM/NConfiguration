using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Configuration.GenericView;

namespace Configuration.MongoDB
{
	internal class ViewString : ICfgNode
	{
		private IPlainConverter _converter;
		private string _text;

		public ViewString(IPlainConverter converter, string text)
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
			return _converter.Convert<string, T>(_text);
		}

		public IEnumerable<KeyValuePair<string, ICfgNode>> GetNodes()
		{
			yield break;
		}
	}
}

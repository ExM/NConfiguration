using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using Configuration.GenericView;
using System.Collections.Specialized;

namespace Configuration.Ini
{
	internal class ViewSection: ICfgNode
	{
		private List<KeyValuePair<string, string>> _pairs;
		private IXmlViewConverter _converter;

		public ViewSection(IXmlViewConverter converter, Section section)
		{
			_converter = converter;
			_pairs = section.Pairs;
		}

		public ICfgNode GetChild(string name)
		{
			var value = _pairs
				.Where(p => p.Key == name)
				.Select(p => p.Value)
				.FirstOrDefault();
			if(value != null)
				return new ViewField(_converter, value);

			return null;
		}

		public IEnumerable<ICfgNode> GetCollection(string name)
		{
			foreach (var value in _pairs.Where(p => p.Key == name).Select(p => p.Value))
				yield return new ViewField(_converter, value);
		}

		public T As<T>()
		{
			throw new NotSupportedException("section can't contain value");
		}

		public IEnumerable<KeyValuePair<string, ICfgNode>> GetNodes()
		{
			foreach (var pair in _pairs)
				yield return new KeyValuePair<string, ICfgNode>(pair.Key, new ViewField(_converter, pair.Value));
		}
	}
}


using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Configuration.GenericView
{
	public class ViewPlainField<TSrc>: ICfgNode
	{
		private TSrc _source;
		private IPlainConverter _converter;

		public ViewPlainField(IPlainConverter converter, TSrc source)
		{
			_converter = converter;
			_source = source;
		}

		public ICfgNode GetChild(string name)
		{
			return null;
		}

		public IEnumerable<ICfgNode> GetCollection(string name)
		{
			yield break;
		}

		public TDst As<TDst>()
		{
			return _converter.Convert<TSrc, TDst>(_source);
		}

		public IEnumerable<KeyValuePair<string, ICfgNode>> GetNodes()
		{
			yield break;
		}
	}
}


using System.Collections.Generic;
using System.Linq;

namespace NConfiguration
{
	public class DefaultConfigNodeProvider : BaseConfigNodeProvider
	{
#if NET40
		private IList<KeyValuePair<string, ICfgNode>> _items;
#else
		private IReadOnlyList<KeyValuePair<string, ICfgNode>> _items;
#endif

		private Dictionary<string, List<ICfgNode>> _index;

		public DefaultConfigNodeProvider(IEnumerable<KeyValuePair<string, ICfgNode>> items)
		{
			_items = items.ToList().AsReadOnly();
			_index = CreateIndex();
		}

#if NET40
		public override IList<KeyValuePair<string, ICfgNode>> Items { get { return _items; } }
#else
		public override IReadOnlyList<KeyValuePair<string, ICfgNode>> Items { get { return _items; } }
#endif

		protected override Dictionary<string, List<ICfgNode>> Index { get { return _index; } }
	}
}

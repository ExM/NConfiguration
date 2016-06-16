using NConfiguration.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NConfiguration
{
	public class DefaultConfigNodeProvider : BaseConfigNodeProvider
	{
		private IReadOnlyList<KeyValuePair<string, ICfgNode>> _items;
		private Dictionary<string, List<ICfgNode>> _index;

		public DefaultConfigNodeProvider(IEnumerable<KeyValuePair<string, ICfgNode>> items)
		{
			_items = items.ToList().AsReadOnly();
			_index = CreateIndex();
		}

		public override IReadOnlyList<KeyValuePair<string, ICfgNode>> Items { get { return _items; } }

		protected override Dictionary<string, List<ICfgNode>> Index { get { return _index; } }
	}
}

using NConfiguration.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NConfiguration
{
	public sealed class ChangeableConfigNodeProvider : BaseConfigNodeProvider, IChangeable
	{
		private readonly IChangeable _changeable;
		private IReadOnlyList<KeyValuePair<string, ICfgNode>> _items;
		private Dictionary<string, List<ICfgNode>> _index;

		public ChangeableConfigNodeProvider(IEnumerable<KeyValuePair<string, ICfgNode>> items, IChangeable changeable)
		{
			_changeable = changeable;
			_items = items.ToList().AsReadOnly();
			_index = CreateIndex();
		}

		public override IReadOnlyList<KeyValuePair<string, ICfgNode>> Items { get { return _items; } }

		protected override Dictionary<string, List<ICfgNode>> Index { get { return _index; } }

		public event EventHandler Changed
		{
			add { _changeable.Changed += value; }
			remove { _changeable.Changed -= value; }
		}
	}
}

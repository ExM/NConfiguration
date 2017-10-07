using System;
using System.Collections.Generic;
using System.Linq;

namespace NConfiguration
{
	public abstract class CachedConfigNodeProvider : BaseConfigNodeProvider
	{
#if NET40
		private readonly Lazy<IList<KeyValuePair<string, ICfgNode>>> _list;
#else
		private readonly Lazy<IReadOnlyList<KeyValuePair<string, ICfgNode>>> _list;
#endif

		private readonly Lazy<Dictionary<string, List<ICfgNode>>> _index;

		protected CachedConfigNodeProvider()
		{
#if NET40
			_list = new Lazy<IList<KeyValuePair<string, ICfgNode>>>(() => GetAllNodes().ToList().AsReadOnly());
#else
			_list = new Lazy<IReadOnlyList<KeyValuePair<string, ICfgNode>>>(() => GetAllNodes().ToList().AsReadOnly());
#endif
			_index = new Lazy<Dictionary<string, List<ICfgNode>>>(CreateIndex);
		}

		protected abstract IEnumerable<KeyValuePair<string, ICfgNode>> GetAllNodes();

#if NET40
		public override IList<KeyValuePair<string, ICfgNode>> Items { get { return _list.Value; } }
#else
		public override IReadOnlyList<KeyValuePair<string, ICfgNode>> Items { get { return _list.Value; } }
#endif

		protected override Dictionary<string, List<ICfgNode>> Index { get { return _index.Value; } }
	}
}

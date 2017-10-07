using System.Collections.Generic;

namespace NConfiguration
{
	public interface IConfigNodeProvider
	{
#if NET40
		IList<KeyValuePair<string, ICfgNode>> Items { get; }
#else
		IReadOnlyList<KeyValuePair<string, ICfgNode>> Items { get; }
#endif

		IEnumerable<ICfgNode> ByName(string sectionName);
	}
}

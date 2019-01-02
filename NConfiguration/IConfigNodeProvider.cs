using System.Collections.Generic;

namespace NConfiguration
{
	public interface IConfigNodeProvider
	{
		IReadOnlyList<KeyValuePair<string, ICfgNode>> Items { get; }

		IEnumerable<ICfgNode> ByName(string sectionName);
	}
}

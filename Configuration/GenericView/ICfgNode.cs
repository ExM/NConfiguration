using System.Collections.Generic;

namespace Configuration.GenericView
{
	public interface ICfgNode
	{

		ICfgNode GetChild(string name);
		IEnumerable<ICfgNode> GetCollection(string name);
		IEnumerable<KeyValuePair<string, ICfgNode>> GetNodes();

		T As<T>();
	}
}

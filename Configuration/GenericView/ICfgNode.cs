using System.Collections.Generic;

namespace Configuration.GenericView
{
	/// <summary>
	/// A node in the document of configuration
	/// </summary>
	public interface ICfgNode
	{
		/// <summary>
		/// Returns the first child node with the specified name or null if no match is found.
		/// </summary>
		/// <param name="name">node name is not case-sensitive.</param>
		/// <returns>Returns the first child node with the specified name or null if no match is found.</returns>
		ICfgNode GetChild(string name);

		/// <summary>
		/// Returns the collection of child nodes with the specified name or empty if no match is found.
		/// </summary>
		/// <param name="name">node name is not case-sensitive.</param>
		/// <returns>Returns the collection of child nodes with the specified name or empty if no match is found.</returns>
		IEnumerable<ICfgNode> GetCollection(string name);

		/// <summary>
		/// Gets all the child nodes with their names.
		/// </summary>
		IEnumerable<KeyValuePair<string, ICfgNode>> GetNodes();

		/// <summary>
		/// Converts the value of a node in an instance of the specified type.
		/// </summary>
		/// <typeparam name="T">The required type</typeparam>
		/// <returns>The required instance</returns>
		T As<T>();
	}
}

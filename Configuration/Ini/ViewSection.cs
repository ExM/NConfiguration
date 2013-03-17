using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using Configuration.GenericView;
using System.Collections.Specialized;

namespace Configuration.Ini
{
	/// <summary>
	/// The mapping section in INI-document to nodes of configuration
	/// </summary>
	public class ViewSection: ICfgNode
	{
		private List<KeyValuePair<string, string>> _pairs;
		private IStringConverter _converter;

		/// <summary>
		/// The mapping INI-document to nodes of configuration
		/// </summary>
		/// <param name="converter">string converter into a simple values</param>
		/// <param name="section">section in INI-document</param>
		public ViewSection(IStringConverter converter, Section section)
		{
			_converter = converter;
			_pairs = section.Pairs;
		}

		/// <summary>
		/// Returns the first child node with the specified name or null if no match is found.
		/// </summary>
		/// <param name="name">node name is not case-sensitive</param>
		/// <returns>Returns the first child node with the specified name or null if no match is found.</returns>
		public ICfgNode GetChild(string name)
		{
			var value = _pairs
				.Where(p => NameComparer.Equals(p.Key, name))
				.Select(p => p.Value)
				.FirstOrDefault();
			if(value != null)
				return new ViewPlainField(_converter, value);

			return null;
		}

		/// <summary>
		/// Returns the collection of child nodes with the specified name or empty if no match is found.
		/// </summary>
		/// <param name="name">node name is not case-sensitive.</param>
		/// <returns>Returns the collection of child nodes with the specified name or empty if no match is found.</returns>
		public IEnumerable<ICfgNode> GetCollection(string name)
		{
			foreach (var value in _pairs.Where(p => NameComparer.Equals(p.Key, name)).Select(p => p.Value))
				yield return new ViewPlainField(_converter, value);
		}

		/// <summary>
		/// Throw NotSupportedException.
		/// </summary>
		public T As<T>()
		{
			throw new NotSupportedException("section can't contain value");
		}

		/// <summary>
		/// Gets all the child nodes with their names.
		/// </summary>
		public IEnumerable<KeyValuePair<string, ICfgNode>> GetNodes()
		{
			foreach (var pair in _pairs)
				yield return new KeyValuePair<string, ICfgNode>(pair.Key, new ViewPlainField(_converter, pair.Value));
		}
	}
}


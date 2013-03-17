using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using Configuration.GenericView;

namespace Configuration.Xml
{
	/// <summary>
	/// The mapping XML-document to nodes of configuration
	/// </summary>
	public class XmlViewNode: ICfgNode
	{
		private XElement _element;
		private IStringConverter _converter;

		/// <summary>
		/// The mapping XML-document to nodes of configuration
		/// </summary>
		/// <param name="converter">string converter into a simple values</param>
		/// <param name="element">XML element</param>
		public XmlViewNode(IStringConverter converter, XElement element)
		{
			_converter = converter;
			_element = element;
		}

		/// <summary>
		/// Returns the first child node with the specified name or null if no match is found.
		/// </summary>
		/// <param name="name">node name is not case-sensitive.</param>
		/// <returns>Returns the first child node with the specified name or null if no match is found.</returns>
		public ICfgNode GetChild(string name)
		{
			var attr = _element.Attributes().FirstOrDefault(a => NameComparer.Equals(name, a.Name.LocalName));
			if (attr != null)
				return new ViewPlainField(_converter, attr.Value);

			var el = _element.Elements().FirstOrDefault(e => NameComparer.Equals(name, e.Name.LocalName));
			if (el != null)
				return new XmlViewNode(_converter, el);

			return null;
		}

		/// <summary>
		/// Returns the collection of child nodes with the specified name or empty if no match is found.
		/// </summary>
		/// <param name="name">node name is not case-sensitive.</param>
		/// <returns>Returns the collection of child nodes with the specified name or empty if no match is found.</returns>
		public IEnumerable<ICfgNode> GetCollection(string name)
		{
			foreach(var attr in _element.Attributes().Where(a => NameComparer.Equals(name, a.Name.LocalName)))
				yield return new ViewPlainField(_converter, attr.Value);

			foreach(var el in _element.Elements().Where(e => NameComparer.Equals(name, e.Name.LocalName)))
				yield return new XmlViewNode(_converter, el);
		}

		/// <summary>
		/// Converts the value of a node in an instance of the specified type.
		/// </summary>
		/// <typeparam name="T">The required type</typeparam>
		/// <returns>The required instance</returns>
		public T As<T>()
		{
			return _converter.Convert<T>(_element.Value);
		}

		/// <summary>
		/// Gets all the child nodes with their names.
		/// </summary>
		public IEnumerable<KeyValuePair<string, ICfgNode>> GetNodes()
		{
			foreach (var attr in _element.Attributes())
				yield return new KeyValuePair<string, ICfgNode>(attr.Name.LocalName, new ViewPlainField(_converter, attr.Value));

			foreach (var el in _element.Elements())
				yield return new KeyValuePair<string, ICfgNode>(el.Name.LocalName, new XmlViewNode(_converter, el));
		}
	}
}


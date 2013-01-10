using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace Configuration.GenericView
{
	internal class XmlViewNode: ICfgNode
	{
		private XElement _element;
		private XmlViewConverter _converter;

		public XmlViewNode(XmlViewConverter converter, XElement element)
		{
			_converter = converter;
			_element = element;
		}

		public ICfgNode GetChild(string name)
		{
			var attr = _element.Attributes().FirstOrDefault(a => name.Equals(a.Name.LocalName, StringComparison.InvariantCultureIgnoreCase));
			if (attr != null)
				return new XmlViewField(_converter, attr.Value);

			var el = _element.Elements().FirstOrDefault(e => name.Equals(e.Name.LocalName, StringComparison.InvariantCultureIgnoreCase));
			if (el != null)
				return new XmlViewNode(_converter, el);

			return null;
		}

		public IEnumerable<ICfgNode> GetCollection(string name)
		{
			foreach(var attr in _element.Attributes().Where(a => name.Equals(a.Name.LocalName, StringComparison.InvariantCultureIgnoreCase)))
				yield return new XmlViewField(_converter, attr.Value);

			foreach(var el in _element.Elements().Where(e => name.Equals(e.Name.LocalName, StringComparison.InvariantCultureIgnoreCase)))
				yield return new XmlViewNode(_converter, el);
		}

		public T As<T>()
		{
			return _converter.Convert<T>(_element.Value);
		}

		public IEnumerable<KeyValuePair<string, ICfgNode>> GetNodes()
		{
			foreach (var attr in _element.Attributes())
				yield return new KeyValuePair<string, ICfgNode>(attr.Name.LocalName, new XmlViewField(_converter, attr.Value));

			foreach (var el in _element.Elements())
				yield return new KeyValuePair<string, ICfgNode>(el.Name.LocalName, new XmlViewNode(_converter, el));
		}
	}
}


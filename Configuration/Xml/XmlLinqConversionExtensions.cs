using System.Collections.Specialized;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Configuration.Xml
{
	/// <summary>
	/// Provides extension methods for simple conversion between System.Xml and System.Xml.Linq classes.
	/// </summary>
	public static class XmlLinqConversionExtensions
	{
		/// <summary>
		/// Converts an XDocument to an XmlDocument.
		/// </summary>
		/// <param name="xdoc">The XDocument to convert.</param>
		/// <returns>The equivalent XmlDocument.</returns>
		public static XmlDocument ToXmlDocument(this XDocument xdoc)
		{
			var xmldoc = new XmlDocument();
			xmldoc.Load(xdoc.CreateReader());
			return xmldoc;
		}
	
		/// <summary>
		/// Converts an XmlDocument to an XDocument.
		/// </summary>
		/// <param name="xmldoc">The XmlDocument to convert.</param>
		/// <returns>The equivalent XDocument.</returns>
		public static XDocument ToXDocument(this XmlDocument xmldoc)
		{
			return XDocument.Load(xmldoc.CreateNavigator().ReadSubtree());
		}
	
		/// <summary>
		/// Converts an XElement to an XmlElement.
		/// </summary>
		/// <param name="xelement">The XElement to convert.</param>
		/// <returns>The equivalent XmlElement.</returns>
		public static XmlElement ToXmlElement(this XElement xelement)
		{
			return new XmlDocument().ReadNode(xelement.CreateReader()) as XmlElement;
		}
	
		/// <summary>
		/// Converts an XmlElement to an XElement.
		/// </summary>
		/// <param name="xmlelement">The XmlElement to convert.</param>
		/// <returns>The equivalent XElement.</returns>
		public static XElement ToXElement(this XmlElement xmlelement)
		{
			return XElement.Load(xmlelement.CreateNavigator().ReadSubtree());
		}

		public static T Deserialize<T>(this XElement element) where T : class
		{
			return XmlDeserializer<T>.Deserialize(element);
		}

		public static NameValueCollection ToNameValueCollection(this XmlAttributeCollection attrs)
		{
			var result = new NameValueCollection();
			foreach(XmlAttribute attr in attrs)
				result.Add(attr.Name, attr.Value);
			return result;
		}
	}
}




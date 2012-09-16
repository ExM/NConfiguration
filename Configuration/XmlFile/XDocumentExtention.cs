using System;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml;

namespace Configuration
{
	/// <summary>
	/// XmlDocument extentions for serialisation of objects in XML elements
	/// </summary>
	public static class XDocumentExtention
	{
		/// <summary>
		/// load object 
		/// </summary>
		/// <param name="elName">element name</param>
		/// <returns>null if element is missing</returns>
		public static T LoadElement<T>(this XDocument doc, string elName) where T : class
		{
			if (doc == null)
				throw new ArgumentNullException("XML document");
			if(elName == null)
				throw new ArgumentNullException("element name");

			XmlSerializer xs = new XmlSerializer(typeof(T), new XmlRootAttribute(elName));
			XElement el = doc.Root.Element(XNamespace.None + elName);
			if (el == null)
				return null;

			using(XmlReader xr = el.CreateReader())
				return (T)xs.Deserialize(xr);
		}
		/// <summary>
		/// replace element
		/// </summary>
		/// <param name="obj">save object</param>
		/// <param name="elName">element name</param>
		public static void SaveElement<T>(this XDocument doc, T obj, string elName) where T : class
		{
			if (doc == null)
				throw new ArgumentNullException("XML document");

			XElement section = Serialize<T>(obj, elName);

			foreach (var el in doc.Root.Elements(section.Name))
				el.Remove();
			doc.Root.Add(section);
		}

		public static XElement Serialize<T>(T obj, string elName) where T : class
		{
			if (elName == null)
				throw new ArgumentNullException("element name");

			XmlSerializer xs = new XmlSerializer(typeof(T), new XmlRootAttribute(elName));
			XDocument doc = new XDocument();
			XmlSerializerNamespaces sn = new XmlSerializerNamespaces(new XmlQualifiedName[] { new XmlQualifiedName(string.Empty) }); // create empty xml namespace
			using (XmlWriter xw = doc.CreateWriter())
				xs.Serialize(xw, obj, sn);
			XElement result = doc.Root;
			XAttribute xmlnsAt = result.Attribute("xmlns");
			if (xmlnsAt != null)
				xmlnsAt.Remove();

			return result;
		}
	}
}

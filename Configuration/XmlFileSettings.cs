using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Configuration
{
	/// <summary>
	/// settings loaded from a file
	/// </summary>
	public class XmlFileSettings : IAppSettings
	{
		private XDocument _doc;

		/// <summary>
		/// settings loaded from a file
		/// </summary>
		/// <param name="fileName">file name</param>
		public XmlFileSettings(string fileName)
		{
			using(var s = System.IO.File.OpenText(fileName))
				_doc = XDocument.Load(s);
		}
		
		/// <summary>
		/// load section
		/// </summary>
		public T Load<T>(string sectionName) where T : class
		{
			if(sectionName == null)
				throw new ArgumentNullException("sectionName");

			XmlSerializer xs = new XmlSerializer(typeof(T), new XmlRootAttribute(sectionName));
			XElement el = _doc.Root.Element(XNamespace.None + sectionName);
			if(el == null)
				throw new SectionNotFoundException(sectionName);
			
			using(XmlReader xr = el.CreateReader())
				return (T)xs.Deserialize(xr);
		}
	}
}


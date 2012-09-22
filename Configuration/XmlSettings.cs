using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Configuration
{
	/// <summary>
	/// settings loaded from a XML document
	/// </summary>
	public abstract class XmlSettings : IAppSettings
	{
		private XDocument _doc;

		protected XmlSettings(XDocument doc)
		{
			if(doc == null)
				throw new ArgumentNullException("doc");
			if(doc.Root == null)
				throw new FormatException("root element not found");
			_doc = doc;
		}
		
		protected XElement GetSectionElement(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			
			return _doc.Root.Element(XNamespace.None + name);
		}
		
		protected T Deserialize<T>(XElement element) where T : class
		{
			if(element == null)
				return null;
			
			var rootAttr = new XmlRootAttribute();
			rootAttr.ElementName = element.Name.LocalName;
			rootAttr.Namespace = element.Name.NamespaceName;
			
			var xs = new XmlSerializer(typeof(T), rootAttr);
			
			using(XmlReader xr = element.CreateReader())
				return (T)xs.Deserialize(xr);
		}
		
		/// <summary>
		/// Trying to load the configuration.
		/// </summary>
		/// <returns>
		/// Instance of the configuration, or null if no section name
		/// </returns>
		/// <param name='sectionName'>instance of application settings</param>
		/// <typeparam name='T'>type of configuration</typeparam>
		public virtual T TryLoad<T>(string sectionName) where T : class
		{
			return Deserialize<T>(GetSectionElement(sectionName));
		}
	}
}


using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Configuration;

namespace Configuration
{
	public class XmlCryptedSettings : IAppSettings
	{
		private XDocument _doc;
		private Func<string, ProtectedConfigurationProvider> _providerFactory;

		public XmlCryptedSettings(string fileName, Func<string, ProtectedConfigurationProvider> providerFactory)
		{
			using(var s = System.IO.File.OpenText(fileName))
				_doc = XDocument.Load(s);
			
			_providerFactory = providerFactory;
		}
		
		public T TryLoad<T>(string sectionName) where T : class
		{
			if(sectionName == null)
				throw new ArgumentNullException("sectionName");

			var xs = new XmlSerializer(typeof(T), new XmlRootAttribute(sectionName));
			var el = _doc.Root.Element(XNamespace.None + sectionName);
			if(el == null)
				return null;
			
			el = Decrypt(el);
			
			using(XmlReader xr = el.CreateReader())
				return (T)xs.Deserialize(xr);
		}
		
		private XElement Decrypt(XElement el)
		{
			var attr = el.Attribute("configProtectionProvider");
			if(attr == null)
				return el;
			
			var provider = _providerFactory(attr.Value);
			if(provider == null)
				throw new InvalidOperationException(string.Format("protection provider `{0}' not found", attr.Value));

			var encData = el.Element("EncryptedData");
			if(encData == null)
				throw new FormatException(string.Format("element `EncryptedData' not found in element `{0}'", el.Name));

			return ((XmlElement)provider.Decrypt(encData.ToXmlElement())).ToXElement();
		}
	}
}


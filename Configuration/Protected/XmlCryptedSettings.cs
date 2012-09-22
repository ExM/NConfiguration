using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Configuration;

namespace Configuration
{
	public class XmlCryptedSettings : XmlSettings
	{
		private Func<string, ProtectedConfigurationProvider> _providerFactory;

		public XmlCryptedSettings(XDocument doc, Func<string, ProtectedConfigurationProvider> providerFactory)
			:base(doc)
		{
			_providerFactory = providerFactory;
		}
		
		public override T TryLoad<T>(string sectionName)
		{
			var el = GetSectionElement(sectionName);
			el = Decrypt(el);
			return Deserialize<T>(el);
		}
		
		private XElement Decrypt(XElement el)
		{
			if(el == null)
				return null;
			
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


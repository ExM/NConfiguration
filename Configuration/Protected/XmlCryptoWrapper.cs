using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Configuration;

namespace Configuration
{
	public class XmlCryptoWrapper : IXmlSettings
	{
		private IXmlSettings _settings;
		private IXmlCryptoProviders _xmlCryptoProviders;

		public XmlCryptoWrapper(IXmlSettings settings, IXmlCryptoProviders xmlCryptoProviders)
		{
			if(settings == null)
				throw new ArgumentNullException("settings");
			_settings = settings;
			
			if(xmlCryptoProviders == null)
				throw new ArgumentNullException("xmlCryptoProviders");
			_xmlCryptoProviders = xmlCryptoProviders;
		}

		public XElement GetSection(string name)
		{
			return Decrypt(_settings.GetSection(name));
		}

		private XElement Decrypt(XElement el)
		{
			if(el == null)
				return null;
			
			var attr = el.Attribute("configProtectionProvider");
			if(attr == null)
				return el;
			
			var provider = _xmlCryptoProviders.GetProvider(attr.Value);
			if(provider == null)
				throw new InvalidOperationException(string.Format("protection provider `{0}' not found", attr.Value));

			var encData = el.Element("EncryptedData");
			if(encData == null)
				throw new FormatException(string.Format("element `EncryptedData' not found in element `{0}'", el.Name));

			return ((XmlElement)provider.Decrypt(encData.ToXmlElement())).ToXElement();
		}
	}
}


using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Configuration;

namespace Configuration.Xml.Protected
{
	public class Wrapper : IXmlSettings
	{
		private IXmlSettings _settings;
		private IProviderCollection _providers;

		public Wrapper(IXmlSettings settings, IProviderCollection providers)
		{
			if(settings == null)
				throw new ArgumentNullException("settings");
			_settings = settings;

			if (providers == null)
				throw new ArgumentNullException("providers");
			_providers = providers;
		}

		public XElement GetSection(string name)
		{
			return Decrypt(_settings.GetSection(name));
		}

		private static XNamespace cryptDataNS = XNamespace.Get("http://www.w3.org/2001/04/xmlenc#");

		private XElement Decrypt(XElement el)
		{
			if(el == null)
				return null;
			
			var attr = el.Attribute("configProtectionProvider");
			if(attr == null)
				return el;

			var provider = _providers.Get(attr.Value);
			if(provider == null)
				throw new InvalidOperationException(string.Format("protection provider `{0}' not found", attr.Value));

			var encData = el.Element(cryptDataNS + "EncryptedData");
			if(encData == null)
				throw new FormatException(string.Format("element `EncryptedData' not found in element `{0}'", el.Name));

			return ((XmlElement)provider.Decrypt(encData.ToXmlElement())).ToXElement();
		}
	}
}


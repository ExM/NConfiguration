using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using Configuration.Xml.Protected;

namespace Configuration.Xml
{
	/// <summary>
	/// settings loaded from a XML document
	/// </summary>
	public abstract class XmlSettings : IXmlEncryptable, IAppSettings
	{
		protected XElement Root { get; set; }
		private IProviderCollection _providers = null;

		private static readonly XNamespace cryptDataNS = XNamespace.Get("http://www.w3.org/2001/04/xmlenc#");

		public XElement GetSection(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");

			if(Root == null)
				return null;

			var el = Root.Element(XNamespace.None + name);

			if(el == null)
				return null;

			var attr = el.Attribute("configProtectionProvider");
			if(attr == null)
				return el;

			if(_providers == null)
				throw new InvalidOperationException("protection providers not configured");

			var provider = _providers.Get(attr.Value);
			if(provider == null)
				throw new InvalidOperationException(string.Format("protection provider `{0}' not found", attr.Value));

			var encData = el.Element(cryptDataNS + "EncryptedData");
			if(encData == null)
				throw new FormatException(string.Format("element `EncryptedData' not found in element `{0}'", el.Name));

			return ((XmlElement)provider.Decrypt(encData.ToXmlElement())).ToXElement();
		}
		
		public void SetProviderCollection(IProviderCollection collection)
		{
			_providers = collection;
		}

		public T TryLoad<T>(string sectionName) where T : class
		{
			var section = GetSection(sectionName);
			if(section == null)
				return null;

			if(typeof(T) == typeof(XElement))
				return (T)(object)XElement.Parse(section.ToString());

			return section.Deserialize<T>();
		}
	}
}


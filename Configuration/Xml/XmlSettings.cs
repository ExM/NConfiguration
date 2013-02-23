using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Xml.Serialization;
using Configuration.Xml.Protected;
using System.Security.Cryptography;
using Configuration.GenericView;

namespace Configuration.Xml
{
	/// <summary>
	/// settings loaded from a XML document
	/// </summary>
	public abstract class XmlSettings : IXmlEncryptable, IAppSettings
	{
		private static readonly XNamespace cryptDataNS = XNamespace.Get("http://www.w3.org/2001/04/xmlenc#");

		private readonly IStringConverter _converter;
		private readonly IGenericDeserializer _deserializer;
		private IProviderCollection _providers = null;

		protected abstract XElement Root { get; }

		public XmlSettings(IStringConverter converter, IGenericDeserializer deserializer)
		{
			_converter = converter;
			_deserializer = deserializer;
		}

		private IEnumerable<XElement> GetSections(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");

			if(Root == null)
				yield break;

			foreach(var el in Root.Elements(XNamespace.None + name))
				yield return Decrypt(el);
		}

		private XElement Decrypt(XElement el)
		{
			if (el == null)
				return null;

			var attr = el.Attribute("configProtectionProvider");
			if (attr == null)
				return el;

			if (_providers == null)
				throw new InvalidOperationException("protection providers not configured");

			var provider = _providers.Get(attr.Value);
			if (provider == null)
				throw new InvalidOperationException(string.Format("protection provider `{0}' not found", attr.Value));

			var encData = el.Element(cryptDataNS + "EncryptedData");
			if (encData == null)
				throw new FormatException(string.Format("element `EncryptedData' not found in element `{0}'", el.Name));

			var xmlEncData = encData.ToXmlElement();
			XmlElement xmlData;

			try
			{
				xmlData = (XmlElement)provider.Decrypt(xmlEncData);
			}
			catch (SystemException sex)
			{
				throw new CryptographicException(string.Format("can't decrypt the configuration section `{0}'", encData.Name), sex);
			}

			return xmlData.ToXElement();
		}
		
		public void SetProviderCollection(IProviderCollection collection)
		{
			_providers = collection;
		}

		public IEnumerable<T> LoadCollection<T>(string sectionName)
		{
			return GetSections(sectionName)
				.Select(el => _deserializer.Deserialize<T>(_converter.CreateView(el)));
		}
	}
}


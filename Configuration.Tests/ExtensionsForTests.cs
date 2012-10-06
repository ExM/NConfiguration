using System.Xml;
using System.Xml.Linq;
using Configuration.Xml.Protected;
using Configuration.Xml;

namespace Configuration
{
	public static class ExtensionsForTests
	{
		public static IAppSettings ToXmlSettings(this string text)
		{
			return new SystemXmlDeserializer(new XmlStringSettings(text));
		}

		public static IAppSettings ToXmlSettings(this string text, IProviderCollection providers)
		{
			IXmlSettings xmlSettings = new XmlStringSettings(text);
			xmlSettings = new Wrapper(xmlSettings, providers);
			return new SystemXmlDeserializer(xmlSettings);
		}

		public static XmlElement ToXmlElement(this string xml)
		{
			var doc = new XmlDocument();
			doc.LoadXml(xml);
			return doc.DocumentElement;
		}

	}
}




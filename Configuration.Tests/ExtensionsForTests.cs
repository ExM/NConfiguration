using System.Xml;
using System.Xml.Linq;
using Configuration.Xml.Protected;
using Configuration.Xml;
using Configuration.GenericView;
using Configuration.Tests;
using Configuration.Ini;
using System.Collections.Generic;
using Configuration.Ini.Parsing;

namespace Configuration
{
	public static class ExtensionsForTests
	{
		public static ICfgNode ToXmlView(this XDocument doc)
		{
			return Global.PlainConverter.CreateView(doc);
		}

		public static IAppSettingSource ToXmlSettings(this string text)
		{
			return new XmlStringSettings(text);
		}

		public static IniStringSettings ToIniSettings(this string text)
		{
			return new IniStringSettings(text);
		}

		public static List<Section> ToIniSections(this string text)
		{
			var context = new ParseContext();
			context.ParseSource(text);
			return new List<Section>(context.Sections);
		}

		public static IAppSettingSource ToXmlSettings(this string text, IProviderCollection providers)
		{
			var settings = new XmlStringSettings(text);
			settings.SetProviderCollection(providers);
			return settings;
		}

		public static XmlElement ToXmlElement(this string xml)
		{
			var doc = new XmlDocument();
			doc.LoadXml(xml);
			return doc.DocumentElement;
		}

		public static XDocument ToXDocument(this string xml)
		{
			return XDocument.Parse(xml);
		}

		public static ICfgNode ToXmlView(this string xml)
		{
			return ToXmlView(XDocument.Parse(xml));
		}
	}
}




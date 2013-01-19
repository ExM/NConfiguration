using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using Configuration.Joining;
using Configuration.GenericView;

namespace Configuration.Xml
{
	public static class SettingsLoaderExtensions
	{
		public static SettingsLoader LoadXmlFile(this SettingsLoader loader, string fileName)
		{
			return loader.LoadSettings(new XmlFileSettings(fileName, XmlViewConverter.Default, loader.Deserializer));
		}

		public static SettingsLoader LoadXmlFile(this SettingsLoader loader, string fileName, IXmlViewConverter converter)
		{
			return loader.LoadSettings(new XmlFileSettings(fileName, converter, loader.Deserializer));
		}

		public static SettingsLoader LoadConfigSection(this SettingsLoader loader, string sectionName)
		{
			return loader.LoadSettings(new XmlSystemSettings(sectionName, XmlViewConverter.Default, loader.Deserializer));
		}

		public static SettingsLoader LoadConfigSection(this SettingsLoader loader, string sectionName, IXmlViewConverter converter)
		{
			return loader.LoadSettings(new XmlSystemSettings(sectionName, converter, loader.Deserializer));
		}

		public static SettingsLoader IncludeInXml(this SettingsLoader loader, IAppSettings settings, params Func<XElement, IAppSettingSource>[] includeHandlers)
		{
			var rootEl = settings.TryLoad<XElement>("Include", false);
			if (rootEl == null)
				return loader;

			var results = new List<IAppSettingSource>();

			foreach (var childEl in rootEl.Elements())
			{
				IAppSettingSource incSettings = null;
				foreach (var handler in includeHandlers)
				{
					incSettings = handler(childEl);
					if (incSettings != null)
						break;
				}

				if (incSettings == null)
					throw new ApplicationException(string.Format("unexpected XML element '{0}'", childEl.Name));
				results.Add(incSettings);
			}

			foreach (var item in results)
				loader.LoadSettings(item);

			return loader;
		}
	}
}


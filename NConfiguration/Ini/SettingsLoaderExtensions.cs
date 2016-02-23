using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using NConfiguration.Joining;
using NConfiguration.Serialization;

namespace NConfiguration.Ini
{
	public static class SettingsLoaderExtensions
	{
		public static FileSearcher IniFileByExtension(this SettingsLoader loader)
		{
			var searcher = new FileSearcher(IniFileSettings.Create, "ini");
			loader.AddHandler<IncludeFileConfig>("IncludeFile", searcher);
			return searcher;
		}

		public static FileSearcher IniFileBySection(this SettingsLoader loader)
		{
			var searcher = new FileSearcher(IniFileSettings.Create);
			loader.AddHandler<IncludeFileConfig>("IncludeIniFile", searcher);
			return searcher;
		}
	}
}


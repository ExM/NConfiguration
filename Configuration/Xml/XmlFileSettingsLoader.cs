using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using Configuration.Joining;
using Configuration.Xml.ConfigSections;

namespace Configuration.Xml.Joining
{
	public static class XmlFileSettingsLoader
	{
		private static readonly XName _elementName = XName.Get("XmlFile", "");

		public static void ResolveXmlElement(object sender, IncludeXmlElementEventArgs args)
		{
			if (args.Handled)
				return;

			if (args.IncludeElement.Name != _elementName)
				return;

			args.Handled = true;

			var rpo = args.BaseSettings as IFilePathOwner;
			var cfg = args.IncludeElement.Deserialize<IncludeFileConfig>();


			if (Path.IsPathRooted(cfg.Path))
			{
				if (File.Exists(cfg.Path) || cfg.Required)
					args.Add(new XmlFileSettings(cfg.Path));
			}
			else
			{
				if (rpo == null)
					throw new InvalidOperationException("can not be searched for a relative path because the settings do not provide an absolute path");

				var found = SearchXmlSettings(rpo.Path, cfg.Path, cfg.Search);

				if (found.Count == 0)
				{
					if (cfg.Required)
						throw new ApplicationException(string.Format("XML configuration '{0}' not found in '{1}'", cfg.Path, rpo.Path));
					else
						return;
				}

				if (cfg.Include == IncludeMode.First)
					args.Add(found.First());
				else if (cfg.Include == IncludeMode.Last)
					args.Add(found.Last());
				else
					foreach (var item in found)
						args.Add(item);
			}
		}

		private static List<XmlFileSettings> SearchXmlSettings(string basePath, string fileName, SearchMode mode)
		{
			var result = new List<XmlFileSettings>();

			if (mode == SearchMode.Up)
				basePath = Path.GetDirectoryName(basePath);

			while(true)
			{
				try
				{
					if (!Directory.Exists(basePath))
						break;
					var fullPath = Path.Combine(basePath, fileName);
					if (File.Exists(fullPath))
					{
						var item = new XmlFileSettings(fullPath);
						result.Add(item);
						var incCfg = item.TryLoad<IncludeConfig>(false);
						if (incCfg != null && incCfg.FinalSearch)
							break;
					}

					if (mode == SearchMode.Exact)
						break;

					basePath = Path.GetDirectoryName(basePath);
				}
				catch (UnauthorizedAccessException)
				{
					continue;
				}
			}

			return result;
		}
	}
}


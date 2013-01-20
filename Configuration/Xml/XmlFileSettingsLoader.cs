using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using Configuration.Joining;
using Configuration.Xml.ConfigSections;
using Configuration.GenericView;

namespace Configuration.Xml.Joining
{
	public class XmlFileSettingsLoader
	{
		public const string ElementName = "XmlFile";

		private readonly IXmlViewConverter _converter;

		public XmlFileSettingsLoader(IXmlViewConverter converter)
		{
			_converter = converter;
		}

		public IXmlViewConverter ViewConverter
		{
			get
			{
				return _converter;
			}
		}

		public void LoadFile(SettingsLoader loader, string fileName)
		{
			var setting = new XmlFileSettings(fileName, _converter, loader.Deserializer);
			loader.LoadSettings(setting);
		}

		public void ResolveXmlFile(object sender, IncludingEventArgs args)
		{
			if (args.Handled)
				return;

			if (!string.Equals(args.Name, ElementName, StringComparison.InvariantCultureIgnoreCase))
				return;

			args.Settings = new List<IAppSettingSource>();

			var deserializer = ((SettingsLoader)sender).Deserializer;

			var rpo = args.Source as IFilePathOwner;
			var cfg = deserializer.Deserialize<IncludeFileConfig>(args.Config);

			if(Path.IsPathRooted(cfg.Path))
			{
				if (!File.Exists(cfg.Path) && !cfg.Required)
					return;

				args.Settings.Add( new XmlFileSettings(cfg.Path, _converter, deserializer));
				return;
			}

			// relative path
			if (rpo == null)
				throw new InvalidOperationException("can not be searched for a relative path because the settings do not provide an absolute path");

			var found = SearchXmlSettings(rpo.Path, cfg.Path, cfg.Search, deserializer);

			if (found.Count == 0)
			{
				if (cfg.Required)
					throw new ApplicationException(string.Format("XML configuration '{0}' not found in '{1}'", cfg.Path, rpo.Path));
				else
					return;
			}

			if (cfg.Include == IncludeMode.First)
				args.Settings.Add(found.First());
			else if (cfg.Include == IncludeMode.Last)
				args.Settings.Add(found.Last());
			else
				foreach (var item in found)
					args.Settings.Add(item);
		}

		private List<XmlFileSettings> SearchXmlSettings(string basePath, string fileName, SearchMode mode, IGenericDeserializer deserializer)
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
						var item = new XmlFileSettings(fullPath, _converter, deserializer);
						result.Add(item);

						if (item.TryLoad<IncludeConfig>(true).FinalSearch)
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


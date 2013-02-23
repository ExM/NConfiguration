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

namespace Configuration.Joining
{
	public abstract class FileSearcher
	{
		protected FileSearcher(IGenericDeserializer deserializer)
		{
			Deserializer = deserializer;
		}

		public IGenericDeserializer Deserializer { get; private set; }

		public abstract string Tag { get; }

		public abstract IAppSettingSource CreateAppSetting(string path);

		public void ResolveFile(object sender, IncludingEventArgs args)
		{
			if (args.Handled)
				return;

			if(!string.Equals(args.Name, Tag, StringComparison.InvariantCultureIgnoreCase))
				return;

			args.Settings = new List<IAppSettingSource>();

			var rpo = args.Source as IFilePathOwner;
			var cfg = Deserializer.Deserialize<IncludeFileConfig>(args.Config);

			if(Path.IsPathRooted(cfg.Path))
			{
				if (!File.Exists(cfg.Path) && !cfg.Required)
					return;

				args.Settings.Add(CreateAppSetting(cfg.Path));
				return;
			}

			// relative path
			if (rpo == null)
				throw new InvalidOperationException("can not be searched for a relative path because the settings do not provide an absolute path");

			var found = SearchSettings(rpo.Path, cfg.Path, cfg.Search);

			if (found.Count == 0)
			{
				if (cfg.Required)
					throw new ApplicationException(string.Format("XML configuration '{0}' not found in '{1}'", cfg.Path, rpo.Path));

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

		private List<IAppSettingSource> SearchSettings(string basePath, string fileName, SearchMode mode)
		{
			var result = new List<IAppSettingSource>();

			if(basePath == null)
				return result;

			if (mode == SearchMode.Up)
				basePath = Path.GetDirectoryName(basePath);

			if(basePath == null)
				return result;

			while(true)
			{
				try
				{
					if (!Directory.Exists(basePath))
						break;
					var fullPath = Path.Combine(basePath, fileName);
					if (File.Exists(fullPath))
					{
						var item = CreateAppSetting(fullPath);
						result.Add(item);

						if (item.TryFirst<IncludeConfig>(true).FinalSearch)
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


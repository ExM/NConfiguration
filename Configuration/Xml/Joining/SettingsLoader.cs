using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using Configuration.ConfigSections;

namespace Configuration.Xml.Joining
{
	public class SettingsLoader
	{
		private MultiSettings _settings;

		public SettingsLoader()
			: this(new MultiSettings())
		{
		}

		public SettingsLoader(ICombineFactory combineFactory)
			: this(new MultiSettings(combineFactory))
		{
		}

		public SettingsLoader(MultiSettings settings)
		{
			_settings = settings;
		}

		public MultiSettings Settings
		{
			get { return _settings; }
		}

		public event EventHandler<LoadedEventArgs> Loaded;

		private void OnLoaded(IAppSettings settings)
		{
			var copy = Loaded;
			if (copy == null)
				return;

			var args = new LoadedEventArgs() { Settings = settings };
			copy(this, args);
		}

		public static SettingsLoader FromXmlFile(string fileName)
		{
			return new SettingsLoader().LoadXmlFile(fileName);
		}

		public SettingsLoader LoadSettings(IAppSettings settings)
		{
			_settings.Add(settings);
			OnLoaded(settings);
			return this;
		}

		public SettingsLoader LoadXmlFile(string fileName)
		{
			return LoadSettings(new XmlFileSettings(fileName));
		}

		public static SettingsLoader FromConfigSection(string sectionName)
		{
			return new SettingsLoader().LoadConfigSection(sectionName);
		}

		public SettingsLoader LoadConfigSection(string sectionName)
		{
			return LoadSettings(new XmlSystemSettings(sectionName));
		}

		public SettingsLoader IncludeIn(IAppSettings settings)
		{
			var incCfg = settings.TryLoad<IncludeConfig>(false);
			if(incCfg == null)
				return this;

			var rpo = settings as IRelativePathOwner;


			foreach(var fCfg in incCfg.FileConfigs)
				if (Path.IsPathRooted(fCfg.Path))
					LoadSettingsByXmlFile(fCfg.Path, fCfg.Required);
				else
				{
					if (rpo == null)
						throw new InvalidOperationException("can not be searched for a relative path because the settings do not provide an absolute path");
					//TODO

				}

			return this;
		}

		private void LoadSettingsByXmlFile(string fileName, bool required)
		{
			if (!File.Exists(fileName) && !required)
				return;

			LoadSettings(new XmlFileSettings(fileName));
		}
	}
}


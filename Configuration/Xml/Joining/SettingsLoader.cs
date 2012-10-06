using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

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

		public event EventHandler<XmlLoadingEventArgs> XmlLoading;

		private bool OnXmlLoading(ref IXmlSettings settings)
		{
			var copy = XmlLoading;
			if (copy == null)
				return false;

			var args = new XmlLoadingEventArgs() { Canceled = false, Settings = settings };
			copy(this, args);
			settings = args.Settings;
			return args.Canceled;
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

		public SettingsLoader LoadSettings(IXmlSettings xmlSettings)
		{
			if (OnXmlLoading(ref xmlSettings))
				return this;
			return LoadSettings(new SystemXmlDeserializer(xmlSettings));
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
	}
}


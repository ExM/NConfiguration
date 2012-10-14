using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Configuration.Joining
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

			var args = new LoadedEventArgs(settings);
			copy(this, args);
		}
		
		public SettingsLoader LoadSettings(IAppSettings settings)
		{
			_settings.Add(settings);
			OnLoaded(settings);
			return this;
		}
	}
}


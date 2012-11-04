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
		private Dictionary<Type, HashSet<string>> _loaded = new Dictionary<Type, HashSet<string>>();

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

		private bool CheckLoaded(IAppSettings settings)
		{
			Type type = settings.GetType();
			HashSet<string> identityLoaded;
			if (_loaded.TryGetValue(type, out identityLoaded))
				return !identityLoaded.Add(settings.Identity);

			identityLoaded = new HashSet<string>();
			identityLoaded.Add(settings.Identity);
			_loaded.Add(type, identityLoaded);
			return false;
		}
	}
}


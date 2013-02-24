using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using Configuration.GenericView;

namespace Configuration.Joining
{
	public class SettingsLoader
	{
		private MultiSettings _settings;
		private HashSet<IdentityKey> _loaded = new HashSet<IdentityKey>();

		public SettingsLoader()
			: this(new MultiSettings())
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

		private void OnLoaded(IAppSettingSource settings)
		{
			var copy = Loaded;
			if (copy != null)
				copy(this, new LoadedEventArgs(settings));
		}

		public SettingsLoader LoadSettings(IAppSettingSource setting)
		{
			if (CheckLoaded(setting))
				return this;
			
			_settings.Add(setting);
			OnLoaded(setting);
			IncludeSettings(setting);
			return this;
		}

		public event EventHandler<IncludingEventArgs> Including;

		private List<IAppSettingSource> OnIncluding(IAppSettingSource source, string name, ICfgNode cfg)
		{
			var copy = Including;
			if (copy != null)
			{
				var args = new IncludingEventArgs(source, name, cfg);
				copy(this, args);
				if (args.Handled)
					return args.Settings;
			}

			throw new InvalidOperationException(string.Format("unknown include type '{0}'", name));
		}

		private void IncludeSettings(IAppSettingSource setting)
		{
			var includeRoot = setting.TryFirst<ICfgNode>("Include", false);
			if(includeRoot == null)
				return;

			foreach (var incNode in includeRoot.GetNodes())
			{
				if (NameComparer.Equals(incNode.Key, "FinalSearch"))
					continue;

				var incSettings = OnIncluding(setting, incNode.Key, incNode.Value);
				if (incSettings != null)
					foreach(var incSetting in incSettings)
						LoadSettings(incSetting);
			}
		}

		private bool CheckLoaded(IAppSettingSource settings)
		{
			var key = new IdentityKey(settings.GetType(), settings.Identity);
			return !_loaded.Add(key);
		}

		private class IdentityKey
		{
			private Type _type;
			private string _id;

			public IdentityKey(Type type, string id)
			{
				_type = type;
				_id = id;
			}

			public bool Equals(IdentityKey other)
			{
				if (other == null)
					return false;

				if (_type != other._type)
					return false;

				return string.Equals(_id, other._id, StringComparison.InvariantCulture);
			}

			public override bool Equals(object obj)
			{
				return Equals(obj as IdentityKey);
			}

			public override int GetHashCode()
			{
				return _type.GetHashCode() ^ _id.GetHashCode();
			}
		}
	}
}


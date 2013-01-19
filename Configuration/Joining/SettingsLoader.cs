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
		private IGenericDeserializer _deserializer;
		private MultiSettings _settings;
		private HashSet<IdentityKey> _loaded = new HashSet<IdentityKey>();

		public SettingsLoader()
			: this(new GenericDeserializer(), new MultiSettings())
		{
		}

		public SettingsLoader(IGenericDeserializer deserializer, ICombineFactory combineFactory)
			: this(deserializer, new MultiSettings(combineFactory))
		{
		}

		public SettingsLoader(IGenericDeserializer deserializer, MultiSettings settings)
		{
			_deserializer = deserializer;
			_settings = settings;
		}

		public MultiSettings Settings
		{
			get { return _settings; }
		}

		public IGenericDeserializer Deserializer
		{
			get { return _deserializer; }
		}

		public event EventHandler<LoadedEventArgs> Loaded;

		private void OnLoaded(IAppSettingSource settings)
		{
			var copy = Loaded;
			if (copy == null)
				return;

			var args = new LoadedEventArgs(settings);
			copy(this, args);
		}

		public SettingsLoader LoadSettings(IAppSettingSource settings)
		{
			if (CheckLoaded(settings))
				return this;
			
			_settings.Add(settings);
			OnLoaded(settings);
			return this;
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


using System;
using System.Linq;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Configuration;
using System.Collections.Specialized;
using Configuration.GenericView;
using System.Collections.Generic;

namespace Configuration.Joining
{
	public class IncludingEventArgs : EventArgs
	{
		public IAppSettingSource Source { get; private set; }
		public string Name { get; private set; }
		public ICfgNode Config { get; private set; }

		private List<IAppSettingSource> _settings = new List<IAppSettingSource>();
		private bool _handled = false;

		public IncludingEventArgs(IAppSettingSource source, string name, ICfgNode cfg)
		{
			Source = source;
			Name = name;
			Config = cfg;
		}

		public bool IsHandled
		{
			get
			{
				return _handled;
			}
		}

		public void Handle()
		{
			if (_handled)
				throw new InvalidOperationException("event already handled");
			_handled = true;
		}

		public List<IAppSettingSource> Settings
		{
			get
			{
				return _settings.ToList();
			}
		}

		public void Add(IAppSettingSource settings)
		{
			_settings.Add(settings);
		}
	}
}


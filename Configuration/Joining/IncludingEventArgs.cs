using System;
using System.Collections.Generic;
using System.Linq;
using Configuration.GenericView;

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


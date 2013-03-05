using System;
using System.Collections.Generic;
using System.Linq;
using Configuration.GenericView;

namespace Configuration.Joining
{
	public class IncludingEventArgs : EventArgs
	{
		public IIdentifiedSource Source { get; private set; }
		public string Name { get; private set; }
		public ICfgNode Config { get; private set; }

		private List<IIdentifiedSource> _settings = new List<IIdentifiedSource>();
		private bool _handled = false;

		public IncludingEventArgs(IIdentifiedSource source, string name, ICfgNode cfg)
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

		public List<IIdentifiedSource> Settings
		{
			get
			{
				return _settings.ToList();
			}
		}

		public void Add(IIdentifiedSource settings)
		{
			_settings.Add(settings);
		}
	}
}


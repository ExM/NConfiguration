using System;
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

		private List<IAppSettingSource> _settings = null;

		public IncludingEventArgs(IAppSettingSource source, string name, ICfgNode cfg)
		{
			Source = source;
			Name = name;
			Config = cfg;
		}

		public bool Handled { get; private set; }

		public List<IAppSettingSource> Settings
		{
			get
			{
				return _settings;
			}
			set
			{
				if (Handled)
					throw new InvalidOperationException("event already handled");
				Handled = true;
				_settings = value;
			}
		}
	}
}


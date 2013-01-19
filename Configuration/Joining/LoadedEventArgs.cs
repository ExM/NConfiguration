using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Configuration;
using System.Collections.Specialized;

namespace Configuration.Joining
{
	public class LoadedEventArgs : EventArgs
	{
		public IAppSettingSource Settings { get; private set; }

		public LoadedEventArgs(IAppSettingSource settings)
		{
			Settings = settings;
		}
	}
}


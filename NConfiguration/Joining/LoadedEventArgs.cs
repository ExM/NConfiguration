using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Configuration;
using System.Collections.Specialized;

namespace NConfiguration.Joining
{
	public class LoadedEventArgs : EventArgs
	{
		public IIdentifiedSource Settings { get; private set; }

		public LoadedEventArgs(IIdentifiedSource settings)
		{
			Settings = settings;
		}
	}
}


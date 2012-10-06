using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Configuration;
using System.Collections.Specialized;

namespace Configuration.Xml.Joining
{
	public class LoadedEventArgs : EventArgs
	{
		public IAppSettings Settings { get; set; }
	}
}


using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Configuration;
using System.Collections.Specialized;

namespace Configuration.Joining
{
	public class FindingSettingsArgs : EventArgs
	{
		public IAppSettings Source { get; private set; }
		public IncludeFileConfig IncludeFile { get; private set; }

		public FindingSettingsArgs(IAppSettings source, IncludeFileConfig cfg)
		{
			Source = source;
			IncludeFile = cfg;
		}
	}
}


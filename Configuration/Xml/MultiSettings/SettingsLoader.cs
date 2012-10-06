using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Configuration.Xml.Joining
{
	public class SettingsLoader
	{
		private MultiSettings _settings;

		public SettingsLoader()
			: this(new MultiSettings())
		{
		}

		public SettingsLoader(ICombineFactory combineFactory)
			: this(new MultiSettings(combineFactory))
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
	}
}


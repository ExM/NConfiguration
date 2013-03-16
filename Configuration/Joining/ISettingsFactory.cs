using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using Configuration.Joining;
using Configuration.GenericView;

namespace Configuration.Joining
{
	public interface ISettingsFactory
	{
		string Tag { get; }
		IEnumerable<IIdentifiedSource> CreateSettings(IIdentifiedSource source, ICfgNode config);
	}
}


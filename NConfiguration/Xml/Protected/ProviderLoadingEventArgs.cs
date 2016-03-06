using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Configuration;
using System.Collections.Specialized;

namespace NConfiguration.Xml.Protected
{
	public class ProviderLoadingEventArgs : CancelableEventArgs
	{
		public string Name { get; set; }
		public string Type { get; set; }
		public NameValueCollection Parameters { get; set; }
	}
}


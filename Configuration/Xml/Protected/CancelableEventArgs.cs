using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Configuration;
using System.Collections.Specialized;

namespace Configuration.Xml.Protected
{
	public class CancelableEventArgs : EventArgs
	{
		public bool Canceled { get; set; }
	}
}


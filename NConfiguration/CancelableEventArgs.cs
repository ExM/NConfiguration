using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Configuration;
using System.Collections.Specialized;

namespace NConfiguration
{
	/// <summary>
	/// Subscriber may cancel the event the cause.
	/// </summary>
	public class CancelableEventArgs : EventArgs
	{
		/// <summary>
		/// The reason the event is canceled
		/// </summary>
		public bool Canceled { get; set; }
	}
}


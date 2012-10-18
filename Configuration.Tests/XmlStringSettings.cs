using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using Configuration.Xml;

namespace Configuration
{
	public class XmlStringSettings : XmlSettings
	{
		private readonly string _hash;

		public XmlStringSettings(string text)
		{
			_hash = text.GetHashCode().ToString();
			Root = XDocument.Parse(text).Root;
		}

		public override string Identity
		{
			get
			{
				return _hash;
			}
		}
	}
}


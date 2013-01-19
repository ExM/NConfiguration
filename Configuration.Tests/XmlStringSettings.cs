using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using Configuration.Xml;
using Configuration.GenericView;

namespace Configuration
{
	public class XmlStringSettings : XmlSettings, IAppSettingSource
	{
		private readonly XElement _root;
		private readonly string _hash;

		public XmlStringSettings(string text)
			:base(new XmlViewConverter(), new GenericDeserializer())
		{
			_hash = text.GetHashCode().ToString();
			_root = XDocument.Parse(text).Root;
		}

		protected override XElement Root
		{
			get
			{
				return _root;
			}
		}

		public string Identity
		{
			get
			{
				return _hash;
			}
		}
	}
}


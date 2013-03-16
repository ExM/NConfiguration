using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using Configuration.Joining;
using Configuration.GenericView;

namespace Configuration.Xml
{
	public class XmlFileSettingsLoader : FileSearcher
	{
		private readonly IStringConverter _converter;

		public XmlFileSettingsLoader(IGenericDeserializer deserializer, IStringConverter converter)
			: base(deserializer)
		{
			_converter = converter;
		}

		public IIdentifiedSource LoadFile(string fileName)
		{
			return new XmlFileSettings(fileName, _converter, Deserializer);
		}

		public override string Tag
		{
			get
			{
				return "XmlFile";
			}
		}

		public override IIdentifiedSource CreateFileSetting(string path)
		{
			return new XmlFileSettings(path, _converter, Deserializer);
		}
	}
}


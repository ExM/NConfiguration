using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using Configuration.Joining;
using Configuration.Xml.ConfigSections;
using Configuration.GenericView;

namespace Configuration.Xml
{
	public class XmlFileSettingsLoader : FileSearcher
	{
		private readonly IPlainConverter _converter;

		public XmlFileSettingsLoader(IGenericDeserializer deserializer, IPlainConverter converter)
			: base(deserializer)
		{
			_converter = converter;
		}

		public IAppSettingSource LoadFile(string fileName)
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

		public override IAppSettingSource CreateAppSetting(string path)
		{
			return new XmlFileSettings(path, _converter, Deserializer);
		}
	}
}


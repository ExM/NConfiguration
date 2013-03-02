using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using Configuration.Joining;
using Configuration.GenericView;

namespace Configuration.Json
{
	public class JsonFileSettingsLoader : FileSearcher
	{
		private readonly IStringConverter _converter;

		public JsonFileSettingsLoader(IGenericDeserializer deserializer, IStringConverter converter)
			: base(deserializer)
		{
			_converter = converter;
		}

		public IAppSettingSource LoadFile(string path)
		{
			return new JsonFileSettings(path, _converter, Deserializer);
		}

		public override string Tag
		{
			get
			{
				return "JsonFile";
			}
		}

		public override IAppSettingSource CreateAppSetting(string path)
		{
			return new JsonFileSettings(path, _converter, Deserializer);
		}
	}
}


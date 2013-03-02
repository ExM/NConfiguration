using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using Configuration.Joining;
using Configuration.GenericView;

namespace Configuration.Ini
{
	public class IniFileSettingsLoader : FileSearcher
	{
		private readonly IStringConverter _converter;

		public IniFileSettingsLoader(IGenericDeserializer deserializer, IStringConverter converter)
			: base(deserializer)
		{
			_converter = converter;
		}

		public IAppSettingSource LoadFile(string path)
		{
			return new IniFileSettings(path, _converter, Deserializer);
		}

		public override string Tag
		{
			get
			{
				return "IniFile";
			}
		}

		public override IAppSettingSource CreateAppSetting(string path)
		{
			return new IniFileSettings(path, _converter, Deserializer);
		}
	}
}


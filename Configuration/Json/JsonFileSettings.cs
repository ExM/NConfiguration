using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using Configuration.Xml.Protected;
using Configuration.GenericView;
using System.Collections.Generic;
using Configuration.Json.Parsing;

namespace Configuration.Json
{
	public class JsonFileSettings : JsonSettings, IFilePathOwner, IAppSettingSource
	{
		private readonly JObject _obj;
		public JsonFileSettings(string fileName, IStringConverter converter, IGenericDeserializer deserializer)
			: base(converter, deserializer)
		{
			try
			{
				fileName = System.IO.Path.GetFullPath(fileName);
				var text = System.IO.File.ReadAllText(fileName);

				var val = JValue.Parse(text);
				if (val.Type != TokenType.Object)
					throw new FormatException("required json object in content");

				_obj = (JObject)val;

				Identity = this.GetIdentitySource(fileName);
				Path = System.IO.Path.GetDirectoryName(fileName);
			}
			catch(SystemException ex)
			{
				throw new ApplicationException(string.Format("Unable to load file `{0}'", fileName), ex);
			}
		}

		protected override IEnumerable<JValue> GetValue(string name)
		{
			return _obj.Properties
				.Where(p => NameComparer.Equals(p.Key, name))
				.Select(p => p.Value);
		}

		public string Identity { get; private set; }

		public string Path { get; private set; }
	}
}


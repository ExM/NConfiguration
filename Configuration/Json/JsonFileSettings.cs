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
		private readonly string _directory;
		private readonly string _identity;

		public JsonFileSettings(string fileName, IPlainConverter converter, IGenericDeserializer deserializer)
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

				_identity = GetCustomIdentity();
				if (_identity == null)
					_identity = fileName;

				_directory = System.IO.Path.GetDirectoryName(fileName);
			}
			catch(SystemException ex)
			{
				throw new ApplicationException(string.Format("Unable to load file `{0}'", fileName), ex);
			}
		}

		private string GetCustomIdentity()
		{
			var val = _obj.Properties.Where(p => p.Key == "Identity").Select(p => p.Value).FirstOrDefault();
			if (val == null)
				return null;

			if (val.Type != TokenType.String ||
				val.Type != TokenType.Number)
				return null;

			return val.ToString();
		}

		protected override IEnumerable<JValue> GetValue(string name)
		{
			return _obj.Properties
				.Where(p => p.Key == name)
				.Select(p => p.Value);
		}
		
		public string Identity
		{
			get
			{
				return _identity;
			}
		}

		public string Path
		{
			get
			{
				return _directory;
			}
		}
	}
}


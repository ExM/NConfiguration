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
using Configuration.Tests;

namespace Configuration.Json
{
	public class JsonStringSettings : JsonSettings
	{
		private readonly JObject _obj;

		public JsonStringSettings(string text)
			: base(Global.PlainConverter, Global.GenericDeserializer)
		{
			var val = JValue.Parse(text);
			if (val.Type != TokenType.Object)
				throw new FormatException("required json object in content");

			_obj = (JObject)val;
		}

		protected override IEnumerable<JValue> GetValue(string name)
		{
			return _obj.Properties
				.Where(p => p.Key == name)
				.Select(p => p.Value);
		}
	}
}


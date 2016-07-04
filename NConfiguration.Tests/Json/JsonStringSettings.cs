using System;
using NConfiguration.Json.Parsing;

namespace NConfiguration.Json
{
	public class JsonStringSettings : JsonSettings
	{
		private readonly JObject _obj;

		public JsonStringSettings(string text)
		{
			var val = JValue.Parse(text);
			if (val.Type != TokenType.Object)
				throw new FormatException("required json object in content");

			_obj = (JObject)val;
		}

		protected override JObject Root
		{
			get { return _obj; }
		}
	}
}


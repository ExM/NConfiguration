using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Configuration.GenericView;
using Configuration.Json.Parsing;

namespace Configuration.Json
{
	public abstract class JsonSettings : IAppSettings
	{
		private readonly IPlainConverter _converter;
		private readonly IGenericDeserializer _deserializer;

		public JsonSettings(IPlainConverter converter, IGenericDeserializer deserializer)
		{
			_converter = converter;
			_deserializer = deserializer;
		}

		protected abstract IEnumerable<JValue> GetValue(string name);

		public IEnumerable<T> LoadCollection<T>(string name)
		{
			foreach(var val in GetValue(name))
				foreach(var item in ViewObject.FlatArray(val))
					yield return _deserializer.Deserialize<T>(ViewObject.CreateByJsonValue(_converter, item));
		}
	}
}

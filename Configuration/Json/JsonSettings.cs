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
		private readonly IStringConverter _converter;
		private readonly IGenericDeserializer _deserializer;

		public JsonSettings(IStringConverter converter, IGenericDeserializer deserializer)
		{
			_converter = converter;
			_deserializer = deserializer;
		}

		protected abstract IEnumerable<JValue> GetValue(string name);

		/// <summary>
		/// Returns a collection of instances of configurations
		/// </summary>
		/// <typeparam name="T">type of instance of configuration</typeparam>
		/// <param name="name">section name</param>
		public IEnumerable<T> LoadCollection<T>(string name)
		{
			foreach(var val in GetValue(name))
				foreach(var item in ViewObject.FlatArray(val))
					yield return _deserializer.Deserialize<T>(ViewObject.CreateByJsonValue(_converter, item));
		}
	}
}

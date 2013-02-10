using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Configuration.GenericView;
using MongoDB.Bson;

namespace Configuration.MongoDB
{
	public abstract class BsonSettings : IAppSettings
	{
		private readonly IViewConverterFactory _converters;
		private readonly IGenericDeserializer _deserializer;

		public BsonSettings(IViewConverterFactory converters, IGenericDeserializer deserializer)
		{
			_converters = converters;
			_deserializer = deserializer;
		}

		protected abstract IEnumerable<BsonDocument> GetDocuments(string name);

		public IEnumerable<T> LoadCollection<T>(string sectionName)
		{
			return GetDocuments(sectionName)
				.Select(d => _deserializer.Deserialize<T>(new ViewDocument(_converters, d)));
		}
	}
}

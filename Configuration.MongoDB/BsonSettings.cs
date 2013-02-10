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
		private readonly IPlainConverter _converter;
		private readonly IGenericDeserializer _deserializer;

		public BsonSettings(IPlainConverter converter, IGenericDeserializer deserializer)
		{
			_converter = converter;
			_deserializer = deserializer;
		}

		protected abstract IEnumerable<BsonDocument> GetDocuments(string name);

		public IEnumerable<T> LoadCollection<T>(string sectionName)
		{
			return GetDocuments(sectionName)
				.Select(d => _deserializer.Deserialize<T>(new ViewDocument(_converter, d)));
		}
	}
}

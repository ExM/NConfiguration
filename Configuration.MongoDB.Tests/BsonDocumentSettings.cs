using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Configuration.GenericView;
using MongoDB.Bson;

namespace Configuration.MongoDB
{
	public class BsonDocumentSettings: BsonSettings
	{
		private List<KeyValuePair<string, BsonDocument>> _docs = new List<KeyValuePair<string, BsonDocument>>();

		public BsonDocumentSettings(IPlainConverter converter, IGenericDeserializer deserializer)
			: base(converter, deserializer)
		{
		}

		public void Add(KeyValuePair<string, BsonDocument> pair)
		{
			_docs.Add(pair);
		}

		protected override IEnumerable<BsonDocument> GetDocuments(string name)
		{
			return _docs.Where(p => p.Key == name).Select(p => p.Value);
		}
	}
}

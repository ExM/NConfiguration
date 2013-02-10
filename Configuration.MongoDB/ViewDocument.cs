using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using Configuration.GenericView;

namespace Configuration.MongoDB
{
	public class ViewDocument: ICfgNode
	{
		private IPlainConverter _converter;
		private BsonDocument _doc;

		public ViewDocument(IPlainConverter converter, BsonDocument doc)
		{
			_converter = converter;
			_doc = doc;
		}

		public ICfgNode GetChild(string name)
		{
			var el = _doc.GetElement(name);
			if (el == null)
				return null;

			return CreateByBsonValue(_converter, FlatArray(el.Value).FirstOrDefault());
		}

		public IEnumerable<ICfgNode> GetCollection(string name)
		{
			return _doc.Elements.Where(el => el.Name == name)
				.SelectMany(el => FlatArray(el.Value))
				.Select(el => CreateByBsonValue(_converter, el));
		}

		public IEnumerable<KeyValuePair<string, ICfgNode>> GetNodes()
		{
			foreach(var el in _doc.Elements)
				foreach (var val in FlatArray(el.Value))
					yield return new KeyValuePair<string, ICfgNode>(el.Name, CreateByBsonValue(_converter, val));
		}

		public T As<T>()
		{
			throw new NotSupportedException("BsonDocument can't contain value");
		}

		internal static ICfgNode CreateByBsonValue(IPlainConverter converter, BsonValue val)
		{
			if (val == null)
				return null;

			switch (val.BsonType)
			{
				case BsonType.Null:
					return null;

				case BsonType.Document:
					return new ViewDocument(converter, (BsonDocument)val);

				case BsonType.String:
					return new ViewPlainField<string>(converter, ((BsonString)val).Value);

				case BsonType.Binary:
					return new ViewPlainField<byte[]>(converter, ((BsonBinaryData)val).Bytes);

				case BsonType.Boolean:
					return new ViewPlainField<bool>(converter, ((BsonBoolean)val).Value);

				case BsonType.DateTime:
					return new ViewPlainField<DateTime>(converter, ((BsonDateTime)val).Value);

				case BsonType.Double:
					return new ViewPlainField<double>(converter, ((BsonDouble)val).Value);

				case BsonType.Int32:
					return new ViewPlainField<int>(converter, ((BsonInt32)val).Value);

				case BsonType.Int64:
					return new ViewPlainField<long>(converter, ((BsonInt64)val).Value);

				case BsonType.JavaScript:
					return new ViewPlainField<string>(converter, ((BsonJavaScript)val).Code);

				case BsonType.RegularExpression:
					return new ViewPlainField<string>(converter, ((BsonRegularExpression)val).ToString());

				case BsonType.Symbol:
					return new ViewPlainField<string>(converter, ((BsonSymbol)val).Name);

				case BsonType.Timestamp:
					return new ViewPlainField<long>(converter, ((BsonTimestamp)val).Value);

				default:
					throw new NotSupportedException(string.Format("BSON type {0} not supported", val.BsonType));
			}
		}

		internal static IEnumerable<BsonValue> FlatArray(BsonValue val)
		{
			if (val == null || val.BsonType == BsonType.Null)
				yield break;

			if (val.BsonType != BsonType.Array)
			{
				yield return val;
				yield break;
			}

			foreach (var item in ((BsonArray)val).Values)
			{
				foreach (var innerItem in FlatArray(item)) //HACK: remove recursion
				{
					yield return innerItem;
				}
			}
		}
	}
}

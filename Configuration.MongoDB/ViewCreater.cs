using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Configuration.GenericView;
using MongoDB.Bson;

namespace Configuration.MongoDB
{
	internal static class ViewCreater
	{
		public static ICfgNode CreateByBsonValue(IViewConverterFactory converters, BsonValue val)
		{
			if (val == null)
				return null;

			switch (val.BsonType)
			{
				case BsonType.Null:
					return null;

				case BsonType.Document:
					return new ViewDocument(converters, (BsonDocument)val);

				case BsonType.String:
					return new ViewString(converters.GetConverter<string>(), ((BsonString)val).Value);


				default:
					throw new NotSupportedException(string.Format("BSON type {0} not supported", val.BsonType));
			}
		}

		public static IEnumerable<BsonValue> FlatArray(BsonValue val)
		{
			if (val == null)
				yield break;

			if (val.BsonType != BsonType.Array)
				yield return val;

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

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
		private IViewConverterFactory _converters;
		private BsonDocument _doc;

		public ViewDocument(IViewConverterFactory converters, BsonDocument doc)
		{
			_converters = converters;
			_doc = doc;
		}

		public ICfgNode GetChild(string name)
		{
			var el = _doc.GetElement(name);
			if (el == null)
				return null;

			return ViewCreater.CreateByBsonValue(_converters, ViewCreater.FlatArray(el.Value).FirstOrDefault());
		}

		public IEnumerable<ICfgNode> GetCollection(string name)
		{
			return _doc.Elements.Where(el => el.Name == name)
				.SelectMany(el => ViewCreater.FlatArray(el.Value))
				.Select(el => ViewCreater.CreateByBsonValue(_converters, el));
		}

		public IEnumerable<KeyValuePair<string, ICfgNode>> GetNodes()
		{
			foreach(var el in _doc.Elements)
				foreach(var val in ViewCreater.FlatArray(el.Value))
					yield return new KeyValuePair<string, ICfgNode>(el.Name, ViewCreater.CreateByBsonValue(_converters, val));
		}

		public T As<T>()
		{
			throw new NotSupportedException("BsonDocument can't contain value");
		}
	}
}

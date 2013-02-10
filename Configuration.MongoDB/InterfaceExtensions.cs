using System;
using System.Linq;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Configuration.GenericView;
using MongoDB.Bson;

namespace Configuration.MongoDB
{
	public static class InterfaceExtensions
	{
		public static ICfgNode CreateView(this BsonDocument doc, IPlainConverter converter)
		{
			return new ViewDocument(converter, doc);
		}
	}
}


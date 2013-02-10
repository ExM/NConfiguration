using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MongoDB.Bson;

namespace Configuration.MongoDB.Tests
{
	[TestFixture]
	public class BsonDocumentViewTests
	{
		[Test]
		public void View()
		{
			BsonDocument doc = new BsonDocument {
				{ "key1", "value1" },
				{ "key2", BsonNull.Value}
			};


			

		}
	}
}

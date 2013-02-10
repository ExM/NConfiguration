using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MongoDB.Bson;
using Configuration.Tests;

namespace Configuration.MongoDB
{
	[TestFixture]
	public class BsonDocumentViewTests
	{
		[Test]
		public void SimpleNavigate()
		{
			BsonDocument doc = new BsonDocument {
				{ "Item1", "item1.value1" },
				{ "Item2", "item2.value1" },
				{ "nullItem", BsonNull.Value}
			};

			var root = doc.CreateView(Global.PlainConverter);

			Assert.IsNull(root.GetChild("nullItem"));
			Assert.IsNotNull(root.GetChild("Item1"));
			Assert.AreEqual("item1.value1", root.GetChild("Item1").As<string>());
		}

		[Test]
		public void ArrayNavigate()
		{
			BsonDocument doc = new BsonDocument(true) {
				{ "Item1", "item1.value1" },
				{ "Item1", new BsonArray { 1, 2 } },
				{ "Item1", BsonNull.Value },
				{ "Item1", new BsonArray { BsonNull.Value, new BsonArray { 1, 2 } } }
			};

			var root = doc.CreateView(Global.PlainConverter);
			Assert.AreEqual("item1.value1", root.GetChild("Item1").As<string>());

			Assert.That(root.GetCollection("Item1").Select(n => n.As<string>()),
				Is.EquivalentTo(new string[] { "item1.value1" , "1", "2", "1", "2"}));
		}
	}
}

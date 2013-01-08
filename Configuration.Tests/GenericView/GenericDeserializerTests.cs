using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using NUnit.Framework;
using Configuration.GenericView.Deserialization;

namespace Configuration.GenericView
{
	[TestFixture]
	public class GenericDeserializerTests
	{
		public struct TestStruct
		{
			public string TextField;
			public string TextProp {get; set;}
		}

		public class TestClass
		{
			public string TextField;
			public string TextProp { get; set; }
		}

		[Test]
		public void ParsePrimitiveType()
		{
			var root = XmlView.Create("<Config TextField='val1' TextProp='val2' />".ToXDocument());
			var d = new GenericDeserializer();

			var ts = d.Deserialize<TestStruct>(root);
			Assert.AreEqual("val1", ts.TextField);
			Assert.AreEqual("val2", ts.TextProp);

			var tc = d.Deserialize<TestClass>(root);
			Assert.AreEqual("val1", tc.TextField);
			Assert.AreEqual("val2", tc.TextProp);
		}
	}
}

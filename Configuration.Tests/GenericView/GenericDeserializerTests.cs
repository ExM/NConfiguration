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
		public struct TestClass
		{
			public string TextField;
			public string TextProp {get; set;}
			public byte[] Dump { get; set; }
			public TestEn EnProp { get; set; }
			public bool BoolField;
			public bool BoolProp { get; set; }
			public bool? NBoolField;
			public bool? NBoolProp { get; set; }
			public byte ByteField;
			public byte ByteProp { get; set; }
			public byte? NByteField;
			public byte? NByteProp { get; set; }
		}

		public enum TestEn
		{
			One,
			Two
		}

		[Test]
		public void ParsePrimitiveType()
		{
			var root = XmlView.Create(
@"<Config
	TextField='val1' TextProp='val2'
	BoolField='+' BoolProp='false'
	NBoolField='T' NBoolProp=''
	ByteField='123' ByteProp='12'
	NByteField='0' NByteProp=''
	EnProp='One'
><Dump></Dump></Config>".ToXDocument());
			var d = new GenericDeserializer();

			var tc = d.Deserialize<TestClass>(root);

			Assert.AreEqual("val1", tc.TextField);
			Assert.AreEqual("val2", tc.TextProp);

			Assert.AreEqual(true, tc.BoolField);
			Assert.AreEqual(false, tc.BoolProp);
			Assert.AreEqual(true, tc.NBoolField);
			Assert.AreEqual(null, tc.NBoolProp);
			Assert.AreEqual(123, tc.ByteField);
			Assert.AreEqual(12, tc.ByteProp);
			Assert.AreEqual(0, tc.NByteField);
			Assert.AreEqual(null, tc.NByteProp);
			Assert.AreEqual(0, tc.Dump.Length);
			Assert.AreEqual(TestEn.One, tc.EnProp);
		}
	}
}

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
		public class PrimitiveTypeCollection
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

			var tc = d.Deserialize<PrimitiveTypeCollection>(root);

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

		public class ComplexTest
		{
			public int[] Array { get; set; }
			public IEnumerable<sbyte> Coll { get; set; }
			public ComplexTest Inner { get; set; }
			public List<ComplexTest> InnerList { get; set; }
		}

		[Test]
		public void ParseEmptyCollections()
		{
			var root = XmlView.Create(
@"<Root></Root>".ToXDocument());
			var d = new GenericDeserializer();

			var tc = d.Deserialize<ComplexTest>(root);

			CollectionAssert.IsEmpty(tc.Array);
			CollectionAssert.IsEmpty(tc.Coll);
			Assert.IsNull(tc.Inner);
			CollectionAssert.IsEmpty(tc.InnerList);
		}

		[Test]
		public void ParseArray1()
		{
			var root = XmlView.Create(
@"<Root Array='123'></Root>".ToXDocument());
			var d = new GenericDeserializer();

			var tc = d.Deserialize<ComplexTest>(root);

			CollectionAssert.AreEqual(new int[] { 123 }, tc.Array);
		}

		[Test]
		public void ParseArray2()
		{
			var root = XmlView.Create(
@"<Root Array='123'><Array>345</Array></Root>".ToXDocument());
			var d = new GenericDeserializer();

			var tc = d.Deserialize<ComplexTest>(root);

			CollectionAssert.AreEqual(new int[] { 123, 345 }, tc.Array);
		}

		[Test]
		public void ParseCollection()
		{
			var root = XmlView.Create(
@"<Root Coll='5'><Coll>-5</Coll></Root>".ToXDocument());
			var d = new GenericDeserializer();

			var tc = d.Deserialize<ComplexTest>(root);

			CollectionAssert.AreEqual(new sbyte[] { 5, -5 }, tc.Coll);
		}

		[Test]
		public void ParseInner()
		{
			var root = XmlView.Create(
@"<Root><Inner Coll='5'><Coll>-5</Coll></Inner></Root>".ToXDocument());
			var d = new GenericDeserializer();

			var tc = d.Deserialize<ComplexTest>(root);

			Assert.NotNull(tc.Inner);
			CollectionAssert.AreEqual(new sbyte[] { 5, -5 }, tc.Inner.Coll);
		}

		[Test]
		public void ParseInnerList()
		{
			var root = XmlView.Create(
@"<Root><InnerList Coll='5'><Coll>-5</Coll></InnerList><InnerList Coll='6'><Coll>-6</Coll></InnerList></Root>".ToXDocument());
			var d = new GenericDeserializer();

			var tc = d.Deserialize<ComplexTest>(root);

			CollectionAssert.AreEqual(new sbyte[] { 5, -5 }, tc.InnerList[0].Coll);
			CollectionAssert.AreEqual(new sbyte[] { 6, -6 }, tc.InnerList[1].Coll);
		}
	}
}

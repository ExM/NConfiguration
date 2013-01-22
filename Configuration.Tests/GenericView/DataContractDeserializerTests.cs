using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using Configuration.GenericView.Deserialization;
using System.Runtime.Serialization;

namespace Configuration.GenericView
{
	[TestFixture]
	public class DataContractDeserializerTests
	{
		public class TestType1
		{
			[IgnoreDataMember]
			public bool Ignored;

			[DataMember]
			public int? NInt1;
			[DataMember(IsRequired = true)]
			public int? NInt2;
			[DataMember(IsRequired = false)]
			public int? NInt3;

			[DataMember]
			public int Int1;
			[DataMember(IsRequired = true)]
			public int Int2;
			[DataMember(IsRequired = false)]
			public int Int3;

			[DataMember]
			public ICfgNode CfgNode1 { get; set; }
			[DataMember(IsRequired = true)]
			public ICfgNode CfgNode2 { get; set; }

			[DataMember]
			public TestType2 Inner1 { get; set; }
			[DataMember(IsRequired = true)]
			public TestType2 Inner2 { get; set; }
		}

		public class TestType2
		{
			[DataMember(IsRequired = true)]
			public int? NInt;
		}

		[Test]
		[ExpectedException(typeof(FormatException))]
		public void BadParse1()
		{
			var root =
@"<Root></Root>".ToXmlView();
			var d = new GenericDeserializer();

			d.Deserialize<TestType1>(root);
		}

		[Test]
		[ExpectedException(typeof(FormatException))]
		public void BadParse2()
		{
			var root =
@"<Root NInt2=''></Root>".ToXmlView();
			var d = new GenericDeserializer();

			d.Deserialize<TestType1>(root);
		}

		[Test]
		[ExpectedException(typeof(FormatException))]
		public void BadParse3()
		{
			var root =
@"<Root NInt2='' Int2=''></Root>".ToXmlView();
			var d = new GenericDeserializer();

			d.Deserialize<TestType1>(root);
		}

		[Test]
		public void RequiredNullable()
		{
			var root =
@"<Root NInt=''></Root>".ToXmlView();
			var d = new GenericDeserializer();

			var t = d.Deserialize<TestType2>(root);

			Assert.AreEqual(null, t.NInt);
		}

		[Test]
		public void EmptyParse()
		{
			var root =
@"<Root Ignored='true' NInt2='' Int2='123' CfgNode2='' ><Inner2 NInt=''/></Root>".ToXmlView();
			var d = new GenericDeserializer();

			var t = d.Deserialize<TestType1>(root);

			Assert.AreEqual(false, t.Ignored);

			Assert.AreEqual(null, t.Inner1);
			Assert.NotNull(t.Inner2);
			Assert.AreEqual(123, t.Int2);
			Assert.AreEqual(null, t.NInt2);
		}
	}
}

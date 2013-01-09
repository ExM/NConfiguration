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
	public class GenericMapperTests
	{
		public struct EmptyStruct
		{
		}

		public class EmptyClass
		{
		}

		[Test]
		public void CreateFunctionForEmptyStruct()
		{
			var mapper = new GenericMapper();
			var func = (Func<ICfgNode, EmptyStruct>)mapper.CreateFunction(typeof(EmptyStruct), null);
			var result = func(null);

			Assert.AreEqual(new EmptyStruct(), result);
		}

		[Test]
		public void CreateFunctionForEmptyClass()
		{
			var mapper = new GenericMapper();
			var func = (Func<ICfgNode, EmptyClass>)mapper.CreateFunction(typeof(EmptyClass), null);
			var result = func(null);

			Assert.NotNull(result);
		}

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
		public void CreateFunctionForTestStruct()
		{
			var mapper = new GenericMapper();
			mapper.CreateFunction(typeof(TestStruct), null);
		}

		[Test]
		public void CreateFunctionForTestClass()
		{
			var mapper = new GenericMapper();
			mapper.CreateFunction(typeof(TestClass), null);
		}
	}
}

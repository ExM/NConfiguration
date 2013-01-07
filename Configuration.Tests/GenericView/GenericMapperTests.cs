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
			var func = mapper.CreateFunction<EmptyStruct>(null);
			var result = func(null);

			Assert.AreEqual(new EmptyStruct(), result);
		}

		[Test]
		public void CreateFunctionForEmptyClass()
		{
			var mapper = new GenericMapper();
			var func = mapper.CreateFunction<EmptyClass>(null);
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
			mapper.CreateFunction<TestStruct>(null);
		}

		[Test]
		public void CreateFunctionForTestClass()
		{
			var mapper = new GenericMapper();
			mapper.CreateFunction<TestClass>(null);
		}
	}
}

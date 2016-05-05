using NConfiguration.Combination;
using NConfiguration.Combination.Collections;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NConfiguration.Tests.Combination.Collections
{
	[TestFixture]
	public class UnionCombinerTests
	{
		[Test]
		public void UnionCollection()
		{
			var x = new TestClass()
			{
				List = new List<string>(){ "L1" },
				Collection = new List<int>() { 123 }
			};

			var y = new TestClass()
			{
				List = new List<string>() { "L2" },
				Collection = new List<int>() { 321 }
			};

			var combined = DefaultCombiner.Instance.Combine(x, y);

			Assert.That(combined.List, Is.EquivalentTo(new []{ "L1", "L2" }));
			Assert.That(combined.Collection, Is.EquivalentTo(new []{ 123, 321 }));
		}

		public class TestClass
		{
			[Combiner(typeof(Union<string>))]
			public List<string> List;

			[Combiner(typeof(Union<int>))]
			public ICollection<int> Collection;
		}

		[Test]
		public void GenericTypeInAttribute()
		{
			var x = new GenTestClass()
			{
				List = new List<string>() { "L1" },
				Array = new [] { "A1" },
			};

			var y = new GenTestClass()
			{
				List = new List<string>() { "L2" },
				Array = new[] { "A2" },
			};

			var combined = DefaultCombiner.Instance.Combine(x, y);

			Assert.That(combined.List, Is.EquivalentTo(new[] { "L1", "L2" }));
			Assert.That(combined.Array, Is.EquivalentTo(new[] { "A1", "A2" }));
		}

		public class GenTestClass
		{
			[Combiner(typeof(Union<>))]
			public List<string> List;

			[Combiner(typeof(Union<>))]
			public string[] Array;
		}

		[Test]
		public void WrongAttributeType()
		{
			var x = new WrongTestClass();
			var y = new WrongTestClass();

			Assert.Throws<InvalidOperationException>(() => DefaultCombiner.Instance.Combine(x, y));
		}

		public class WrongTestClass
		{
			[Combiner(typeof(Union<string>))]
			public List<string> List;

			[Combiner(typeof(Union<string>))]
			public ICollection<int> Collection;
		}
	}
}

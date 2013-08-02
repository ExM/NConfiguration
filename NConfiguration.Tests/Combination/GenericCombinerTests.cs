using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using NUnit.Framework;
using NConfiguration.GenericView.Deserialization;

namespace NConfiguration.Combination
{
	[TestFixture]
	public class GenericCombinerTests
	{
		private static IGenericCombiner _combiner = new GenericCombiner();

		[TestCase(0, 0, 0)]
		[TestCase(0, 5, 5)]
		[TestCase(5, 0, 5)]
		[TestCase(5, 8, 8)]
		public void CombineInt(int x, int y, int res)
		{
			Assert.That(_combiner.Combine(x, y), Is.EqualTo(res));
		}

		[TestCase("A", "B", "B")]
		[TestCase(null, "B", "B")]
		[TestCase("A", null, "A")]
		[TestCase(null, null, null)]
		public void CombineString(string x, string y, string res)
		{
			Assert.That(_combiner.Combine(x, y), Is.EqualTo(res));
		}

		[TestCase(0, 0, 0)]
		[TestCase(0, 5, 5)]
		[TestCase(5, 0, 0)]
		[TestCase(5, 8, 8)]
		[TestCase(null, 5, 5)]
		[TestCase(5, null, 5)]
		[TestCase(null, null, null)]
		public void CombineNInt(int? x, int? y, int? res)
		{
			Assert.That(_combiner.Combine(x, y), Is.EqualTo(res));
		}
	}
}

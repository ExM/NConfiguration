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
	public class ComplexTests
	{
		private static IGenericCombiner _combiner = new GenericCombiner();

		[Test]
		public void Combine()
		{
			var prev = new TestStruct()
			{
				PString = "prev", FString1 = "prev", FString2 = "prev", FInt = 1, NInt = 0
			};
			
			var next = new TestStruct()
			{
				PString = "next", FString1 = "next", FString2 = null, FInt = 0, NInt = null
			};

			var result = _combiner.Combine(prev, next);
			
			Assert.That(result.PString, Is.EqualTo("next"));
			Assert.That(result.FString1, Is.EqualTo("next"));
			Assert.That(result.FString2, Is.EqualTo("prev"));
			Assert.That(result.FInt, Is.EqualTo(1));
			Assert.That(result.NInt, Is.EqualTo(0));
		}
	}
}

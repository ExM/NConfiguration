using NConfiguration.Combination;

namespace NConfiguration.Tests.Combination.DefaultCombinationTests
{
	public class TestGenericCombinableClass : ICombinable<TestGenericCombinableClass>
	{
		public string F1;
		public int F2;

		public void Combine(ICombiner combiner, TestGenericCombinableClass other)
		{
			if (other == null)
				return;

			F1 += other.F1;
			F2 += other.F2;
		}
	}
}


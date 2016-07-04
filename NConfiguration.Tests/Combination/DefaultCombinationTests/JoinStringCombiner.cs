using NConfiguration.Combination;

namespace NConfiguration.Tests.Combination.DefaultCombinationTests
{
	public class JoinStringCombiner : ICombiner<string>
	{
		public string Combine(ICombiner combiner, string x, string y)
		{
			return (x ?? "null") + "," + (y ?? "null");
		}
	}
}

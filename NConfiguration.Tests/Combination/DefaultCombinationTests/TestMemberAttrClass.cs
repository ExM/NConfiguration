using NConfiguration.Combination;

namespace NConfiguration.Tests.Combination.DefaultCombinationTests
{
	public class TestMemberAttrClass
	{
		public string F1;

		[Combiner(typeof(JoinStringCombiner))]
		public string F1A;

		public string P1 { get; set; }

		[Combiner(typeof(JoinStringCombiner))]
		public string P1A { get; set; }
	}
}


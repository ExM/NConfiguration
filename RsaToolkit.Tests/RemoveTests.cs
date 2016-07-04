using NUnit.Framework;

namespace RsaToolkit
{
	[TestFixture]
	public class RemoveTests : BaseTest
	{
		[Test]
		public void NoContainer()
		{
			"remove".FailRun();
		}

		[Test]
		public void FromContainer()
		{
			"create -n=TestContainer".SuccessRun();

			"remove -n=TestContainer".SuccessRun();

			"export -n=TestContainer -f=testKey.xml".FailRun();
		}
	}
}

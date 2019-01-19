using NUnit.Framework;

namespace RsaToolkit
{
	[TestFixture]
	public class RemoveTests : BaseTest
	{
		[Test]
		public void NoContainer()
		{
			Program.Main("remove").AreFail();
		}

		[Test]
		public void FromContainer()
		{
			Program.Main("create", "-n=TestContainer").AreSuccess();

			Program.Main("remove", "-n=TestContainer").AreSuccess();

			Program.Main("export", "-n=TestContainer", "-f=testKey.xml").AreFail();
		}
	}
}

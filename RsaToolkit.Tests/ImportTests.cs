using NUnit.Framework;

namespace RsaToolkit
{
	[TestFixture]
	public class ImportTests : BaseTest
	{
		[Test]
		public void NoInput()
		{
			"import -n=TestContainer".FailRun();
		}

		[Test]
		public void NoOutput()
		{
			"import -f=testKey.xml".FailRun();
		}

		[Test]
		public void NoXmlFile()
		{
			"remove -n=TestContainer".Run();
			"testKey.xml".DeleteIfExist();
			"import -f=testKey.xml -n=TestContainer".FailRun();

			"remove -n=TestContainer".FailRun(); // container not created
		}

		[Test]
		public void ToContainer()
		{
			"testKey.xml".DeleteIfExist();

			"create -f=testKey.xml".SuccessRun();

			"import -f=testKey.xml -n=TestContainer".SuccessRun();

			"exportedKey.xml".DeleteIfExist();

			"export -n=TestContainer -f=exportedKey.xml".SuccessRun();

			FileAssert.AreEqual("testKey.xml", "exportedKey.xml");
		}
	}
}

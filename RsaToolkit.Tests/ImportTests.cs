using NUnit.Framework;

namespace RsaToolkit
{
	[TestFixture]
	public class ImportTests : BaseTest
	{
		[Test]
		public void NoInput()
		{
			Program.Main("import", "-n=TestContainer").AreFail();
		}

		[Test]
		public void NoOutput()
		{
			Program.Main("import", "-f=testKey.xml").AreFail();
		}

		[Test]
		public void NoXmlFile()
		{
			Program.Main("remove", "-n=TestContainer");
			DeleteIfExist("testKey.xml");
			Program.Main("import", "-f=testKey.xml", "-n=TestContainer").AreFail();

			Program.Main("remove", "-n=TestContainer").AreFail(); // container not created
		}

		[Test]
		public void ToContainer()
		{
			DeleteIfExist("testKey.xml");

			Program.Main("create", "-f=testKey.xml").AreSuccess();

			Program.Main(
				"import"
				,"-f=testKey.xml"
				,"-n=TestContainer"
				//,"-r=\"LOCAL SERVICE;NETWORK SERVICE\""
				).AreSuccess();

			DeleteIfExist("exportedKey.xml");

			Program.Main("export", "-n=TestContainer", "-f=exportedKey.xml").AreSuccess();

			FileAssert.AreEqual("testKey.xml", "exportedKey.xml");
		}
	}
}

using NUnit.Framework;
using System.IO;

namespace RsaToolkit
{
	[TestFixture]
	public class ExportTests : BaseTest
	{
		[Test]
		public void NoOutput()
		{
			Program.Main("export", "-n=TestContainer").AreFail();
		}

		[Test]
		public void NoInput()
		{
			DeleteIfExist("exportedKey.xml");

			Program.Main("export", "-f=exportedKey.xml").AreFail();

			Assert.False(File.Exists("exportedKey.xml"));
		}

		[Test]
		public void ToXmlFile()
		{
			Program.Main("create", "-n=TestContainer").AreSuccess();

			DeleteIfExist("exportedKey1.xml");
			DeleteIfExist("exportedKey2.xml");

			Program.Main("export", "-n=TestContainer", "-f=exportedKey1.xml").AreSuccess();
			Assert.True(File.Exists("exportedKey1.xml"));

			Program.Main("export", "-n=TestContainer", "-f=exportedKey2.xml").AreSuccess();
			Assert.True(File.Exists("exportedKey2.xml"));

			FileAssert.AreEqual("exportedKey1.xml", "exportedKey2.xml");
		}
	}
}

using NUnit.Framework;
using System.IO;

namespace RsaToolkit
{
	[TestFixture]
	public class CreateTests : BaseTest
	{
		[TestCase("512")]
		[TestCase("1024")]
		[TestCase("2048")]
		[TestCase("3072")]
		public void ToXmlFile_KeySize(string keySize)
		{
			DeleteIfExist("testKey" + keySize + ".xml");

			Program.Main("create","-f=testKey" + keySize + ".xml", "-s=" + keySize).AreSuccess();

			Assert.True(File.Exists("testKey" + keySize + ".xml"));
		}

		[Test]
		public void NoOutput()
		{
			DeleteIfExist("testKey.xml");

			Program.Main("create", "-s=1024").AreFail();

			Assert.False(File.Exists("testKey.xml"));
		}

		[Test]
		public void NotNumber()
		{
			DeleteIfExist("testKey.xml");

			Program.Main("create", "-s=ABC", "-f=testKey.xml").AreFail();

			Assert.False(File.Exists("testKey.xml"));
		}

		[Test]
		public void ToXmlFile()
		{
			DeleteIfExist("testKey.xml");

			Program.Main("create","-f=testKey.xml").AreSuccess();;

			Assert.True(File.Exists("testKey.xml"));
		}

		[Test]
		public void ToContainer()
		{
			Program.Main("remove", "-n=TestContainer");

			Program.Main("create", "-n=TestContainer").AreSuccess();
			Program.Main("remove", "-n=TestContainer").AreSuccess();
		}
	}
}

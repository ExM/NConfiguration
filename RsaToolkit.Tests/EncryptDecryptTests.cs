using NUnit.Framework;
using System.IO;
using System.Xml.Linq;

namespace RsaToolkit
{
	[TestFixture]
	public class EncryptDecryptTests : BaseTest
	{

		[Test]
		public void WrongDecrypt()
		{
			File.WriteAllText("testConfig.xml", _xmlConfigContent);

			Program.Main("create", "-n=TestContainer").AreSuccess();

			Program.Main("create", "-n=TestContainer2").AreSuccess();

			Program.Main("encrypt", "-n=TestContainer", "-c=testConfig.xml", "-s=MySecuredConnection", "-p=TestProvider").AreSuccess();

			var cryptedContent = File.ReadAllText("testConfig.xml");

			Assert.AreNotEqual(_xmlConfigContent, cryptedContent);

			Program.Main("decrypt", "-n=TestContainer2", "-c=testConfig.xml", "-s=MySecuredConnection").AreFail();

			Assert.AreEqual(cryptedContent, File.ReadAllText("testConfig.xml"));
		}

		[Test]
		public void FromNewContainer()
		{
			File.WriteAllText("testConfig.xml", _xmlConfigContent);

			Program.Main("create", "-n=TestContainer").AreSuccess();

			Program.Main("encrypt", "-n=TestContainer", "-c=testConfig.xml", "-s=MySecuredConnection", "-p=TestProvider").AreSuccess();

			Assert.AreNotEqual(_xmlConfigContent, File.ReadAllText("testConfig.xml"));

			Program.Main("decrypt", "-n=TestContainer", "-c=testConfig.xml", "-s=MySecuredConnection").AreSuccess();

			Assert.AreEqual(_xmlConfigContent, File.ReadAllText("testConfig.xml"));
		}

		private static string _xmlConfigContent = XDocument.Parse(@"<configuration>
	<AdditionalConfig F='InUpDirectory' />
	<MyExtConnection Server='localhost' Database='workDb' User='admin' Password='pass' />
	<MySecuredConnection Server='localhost' Database='workDb' User='admin' Password='pass' />
</configuration>".Replace('\'', '\"')).ToString();
	}
}

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

			"create -n=TestContainer".SuccessRun();

			"create -n=TestContainer2".SuccessRun();

			"encrypt -n=TestContainer -c=testConfig.xml -s=MySecuredConnection -p=TestProvider".SuccessRun();

			var cryptedContent = File.ReadAllText("testConfig.xml");

			Assert.AreNotEqual(_xmlConfigContent, cryptedContent);

			"decrypt -n=TestContainer2 -c=testConfig.xml -s=MySecuredConnection".FailRun();

			Assert.AreEqual(cryptedContent, File.ReadAllText("testConfig.xml"));
		}

		[Test]
		public void FromNewContainer()
		{
			File.WriteAllText("testConfig.xml", _xmlConfigContent);

			"create -n=TestContainer".SuccessRun();

			"encrypt -n=TestContainer -c=testConfig.xml -s=MySecuredConnection -p=TestProvider".SuccessRun();

			Assert.AreNotEqual(_xmlConfigContent, File.ReadAllText("testConfig.xml"));

			"decrypt -n=TestContainer -c=testConfig.xml -s=MySecuredConnection".SuccessRun();

			Assert.AreEqual(_xmlConfigContent, File.ReadAllText("testConfig.xml"));
		}

		private static string _xmlConfigContent = XDocument.Parse(@"<configuration>
	<AdditionalConfig F='InUpDirectory' />
	<MyExtConnection Server='localhost' Database='workDb' User='admin' Password='pass' />
	<MySecuredConnection Server='localhost' Database='workDb' User='admin' Password='pass' />
</configuration>".Replace('\'', '\"')).ToString();
	}
}

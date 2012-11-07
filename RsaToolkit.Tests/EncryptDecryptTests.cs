using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;

namespace RsaToolkit
{
	[TestFixture]
	public class EncryptDecryptTests
	{

		[Test]
		public void WrongDecrypt()
		{
			File.WriteAllText("testConfig.xml", XmlConfigContent);

			var exCode = "create -n=TestContainer".Run();
			Assert.AreEqual(0, exCode);

			exCode = "create -n=TestContainer2".Run();
			Assert.AreEqual(0, exCode);

			exCode = "encrypt -n=TestContainer -c=testConfig.xml -s=MySecuredConnection -p=TestProvider".Run();
			Assert.AreEqual(0, exCode);

			var cryptedContent = File.ReadAllText("testConfig.xml");

			Assert.AreNotEqual(XmlConfigContent, cryptedContent);

			exCode = "decrypt -n=TestContainer2 -c=testConfig.xml -s=MySecuredConnection".Run();
			Assert.AreEqual(1, exCode);

			Assert.AreEqual(cryptedContent, File.ReadAllText("testConfig.xml"));
		}

		[Test]
		public void FromKeyFile()
		{
			File.WriteAllText("testConfig.xml", XmlConfigContent);

			var exCode = "create -f=testKey.xml".Run();
			Assert.AreEqual(0, exCode);

			exCode = "encrypt -f=testKey.xml -c=testConfig.xml -s=MySecuredConnection -p=TestProvider".Run();
			Assert.AreEqual(0, exCode);

			Assert.AreNotEqual(XmlConfigContent, File.ReadAllText("testConfig.xml"));

			exCode = "decrypt -f=testKey.xml -c=testConfig.xml -s=MySecuredConnection".Run();
			Assert.AreEqual(0, exCode);

			Assert.AreEqual(XmlConfigContent, File.ReadAllText("testConfig.xml"));
		}

		[Test]
		public void FromContainer()
		{
			File.WriteAllText("testConfig.xml", XmlConfigContent);

			var exCode = "create -n=TestContainer".Run();
			Assert.AreEqual(0, exCode);

			exCode = "encrypt -n=TestContainer -c=testConfig.xml -s=MySecuredConnection -p=TestProvider".Run();
			Assert.AreEqual(0, exCode);

			Assert.AreNotEqual(XmlConfigContent, File.ReadAllText("testConfig.xml"));

			exCode = "decrypt -n=TestContainer -c=testConfig.xml -s=MySecuredConnection".Run();
			Assert.AreEqual(0, exCode);

			Assert.AreEqual(XmlConfigContent, File.ReadAllText("testConfig.xml"));
		}

		private static string XmlConfigContent = XDocument.Parse(@"<configuration>
	<AdditionalConfig F='InUpDirectory' />
	<MyExtConnection Server='localhost' Database='workDb' User='admin' Password='pass' />
	<MySecuredConnection Server='localhost' Database='workDb' User='admin' Password='pass' />
</configuration>").ToString();
	}
}

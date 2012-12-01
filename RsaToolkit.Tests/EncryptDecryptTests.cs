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

			"create -n=TestContainer".SuccessRun();

			"create -n=TestContainer2".SuccessRun();

			"encrypt -n=TestContainer -c=testConfig.xml -s=MySecuredConnection -p=TestProvider".SuccessRun();

			var cryptedContent = File.ReadAllText("testConfig.xml");

			Assert.AreNotEqual(XmlConfigContent, cryptedContent);

			"decrypt -n=TestContainer2 -c=testConfig.xml -s=MySecuredConnection".FailRun();

			Assert.AreEqual(cryptedContent, File.ReadAllText("testConfig.xml"));
		}

		[Test]
		public void FromKeyFile()
		{
			File.WriteAllText("testConfig.xml", XmlConfigContent);

			"create -f=testKey.xml".SuccessRun();

			"encrypt -f=testKey.xml -c=testConfig.xml -s=MySecuredConnection -p=TestProvider".SuccessRun();

			Assert.AreNotEqual(XmlConfigContent, File.ReadAllText("testConfig.xml"));

			"decrypt -f=testKey.xml -c=testConfig.xml -s=MySecuredConnection".SuccessRun();

			Assert.AreEqual(XmlConfigContent, File.ReadAllText("testConfig.xml"));
		}

		[Test]
		public void FromContainer()
		{
			File.WriteAllText("testConfig.xml", XmlConfigContent);

			"create -n=TestContainer".SuccessRun();

			"encrypt -n=TestContainer -c=testConfig.xml -s=MySecuredConnection -p=TestProvider".SuccessRun();

			Assert.AreNotEqual(XmlConfigContent, File.ReadAllText("testConfig.xml"));

			"decrypt -n=TestContainer -c=testConfig.xml -s=MySecuredConnection".SuccessRun();

			Assert.AreEqual(XmlConfigContent, File.ReadAllText("testConfig.xml"));
		}

		private static string XmlConfigContent = XDocument.Parse(@"<?xml version='1.0' encoding='utf-8' ?>
<configuration>
	<AdditionalConfig F='InUpDirectory'/>
	<MyExtConnection Server='localhost' Database='workDb' User='admin' Password='pass' />
	<MySecuredConnection Server='localhost' Database='workDb' User='admin' Password='pass' />
</configuration>").ToString();
	}
}

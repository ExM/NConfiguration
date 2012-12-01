using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Diagnostics;
using System.IO;

namespace RsaToolkit
{
	[TestFixture]
	public class CreateTests
	{
		[TestCase("512")]
		[TestCase("1024")]
		[TestCase("2048")]
		[TestCase("3072")]
		public void ToXmlFile_KeySize(string keySize)
		{
			("testKey" + keySize + ".xml").DeleteIfExist();

			("create -f=testKey" + keySize + ".xml -s=" + keySize).SuccessRun();

			Assert.True(File.Exists("testKey" + keySize + ".xml"));
		}

		[Test]
		public void NoOutput()
		{
			"testKey.xml".DeleteIfExist();

			"create -s=1024".FailRun();

			Assert.False(File.Exists("testKey.xml"));
		}

		[Test]
		public void NotNumber()
		{
			"testKey.xml".DeleteIfExist();

			"create -s=ABC -f=testKey.xml".FailRun();

			Assert.False(File.Exists("testKey.xml"));
		}

		[Test]
		public void ToXmlFile()
		{
			"testKey.xml".DeleteIfExist();

			"create -f=testKey.xml".SuccessRun();

			Assert.True(File.Exists("testKey.xml"));
		}

		[Test]
		public void ToContainer()
		{
			"remove -n=TestContainer".Run();

			"create -n=TestContainer".SuccessRun();
			"remove -n=TestContainer".SuccessRun();
		}
	}
}

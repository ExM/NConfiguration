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

			var exCode = ("create -f=testKey" + keySize + ".xml -s=" + keySize).Run();

			Assert.AreEqual(0, exCode);
			Assert.True(File.Exists("testKey" + keySize + ".xml"));
		}

		[Test]
		public void NoOutput()
		{
			"testKey.xml".DeleteIfExist();

			var exCode = "create -s=1024".Run();

			Assert.AreEqual(1, exCode);
			Assert.False(File.Exists("testKey.xml"));
		}

		[Test]
		public void NotNumber()
		{
			"testKey.xml".DeleteIfExist();

			var exCode = "create -s=ABC -f=testKey.xml".Run();

			Assert.AreEqual(1, exCode);
			Assert.False(File.Exists("testKey.xml"));
		}

		[Test]
		public void ToXmlFile()
		{
			"testKey.xml".DeleteIfExist();

			var exCode = "create -f=testKey.xml".Run();

			Assert.AreEqual(0, exCode);
			Assert.True(File.Exists("testKey.xml"));
		}

		[Test]
		public void ToContainer()
		{
			"remove -n=TestContainer".Run();

			var exCode = "create -n=TestContainer".Run();
			Assert.AreEqual(0, exCode);

			exCode = "remove -n=TestContainer".Run();
			Assert.AreEqual(0, exCode);
		}
	}
}

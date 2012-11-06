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
	public class ExportTests
	{
		[Test]
		public void NoOutput()
		{
			var exCode = "export -n=TestContainer".Run();
			Assert.AreEqual(1, exCode);
		}

		[Test]
		public void NoInput()
		{
			"exportedKey.xml".DeleteIfExist();

			var exCode = "export -f=exportedKey.xml".Run();

			Assert.AreEqual(1, exCode);
			Assert.False(File.Exists("exportedKey.xml"));
		}

		[Test]
		public void ToXmlFile()
		{
			var exCode = "create -n=TestContainer".Run();
			Assert.AreEqual(0, exCode);

			"exportedKey1.xml".DeleteIfExist();
			"exportedKey2.xml".DeleteIfExist();

			exCode = "export -n=TestContainer -f=exportedKey1.xml".Run();
			Assert.AreEqual(0, exCode);
			Assert.True(File.Exists("exportedKey1.xml"));

			exCode = "export -n=TestContainer -f=exportedKey2.xml".Run();
			Assert.AreEqual(0, exCode);
			Assert.True(File.Exists("exportedKey2.xml"));

			FileAssert.AreEqual("exportedKey1.xml", "exportedKey2.xml");
		}
	}
}

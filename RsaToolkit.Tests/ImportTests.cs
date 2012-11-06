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
	public class ImportTests
	{
		[Test]
		public void NoInput()
		{
			var exCode = "import -n=TestContainer".Run();
			Assert.AreEqual(1, exCode);
		}

		[Test]
		public void NoOutput()
		{
			var exCode = "import -f=testKey.xml".Run();
			Assert.AreEqual(1, exCode);
		}

		[Test]
		public void NoXmlFile()
		{
			"remove -n=TestContainer".Run();
			"testKey.xml".DeleteIfExist();
			var exCode = "import -f=testKey.xml -n=TestContainer".Run();
			Assert.AreEqual(1, exCode);

			exCode = "remove -n=TestContainer".Run();
			Assert.AreEqual(1, exCode); // container not created
		}

		[Test]
		public void ToContainer()
		{
			"testKey.xml".DeleteIfExist();

			var exCode = "create -f=testKey.xml".Run();
			Assert.AreEqual(0, exCode);

			exCode = "import -f=testKey.xml -n=TestContainer".Run();
			Assert.AreEqual(0, exCode);

			"exportedKey.xml".DeleteIfExist();

			exCode = "export -n=TestContainer -f=exportedKey.xml".Run();
			Assert.AreEqual(0, exCode);

			FileAssert.AreEqual("testKey.xml", "exportedKey.xml");
		}
	}
}

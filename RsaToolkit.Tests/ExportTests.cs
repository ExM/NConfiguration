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
			"export -n=TestContainer".FailRun();
		}

		[Test]
		public void NoInput()
		{
			"exportedKey.xml".DeleteIfExist();

			"export -f=exportedKey.xml".FailRun();

			Assert.False(File.Exists("exportedKey.xml"));
		}

		[Test]
		public void ToXmlFile()
		{
			"create -n=TestContainer".SuccessRun();

			"exportedKey1.xml".DeleteIfExist();
			"exportedKey2.xml".DeleteIfExist();

			"export -n=TestContainer -f=exportedKey1.xml".SuccessRun();
			Assert.True(File.Exists("exportedKey1.xml"));

			"export -n=TestContainer -f=exportedKey2.xml".SuccessRun();
			Assert.True(File.Exists("exportedKey2.xml"));

			FileAssert.AreEqual("exportedKey1.xml", "exportedKey2.xml");
		}
	}
}

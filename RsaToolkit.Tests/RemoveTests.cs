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
	public class RemoveTests
	{
		[Test]
		public void NoContainer()
		{
			var exCode = "remove".Run();
			Assert.AreEqual(1, exCode);
		}

		[Test]
		public void FromContainer()
		{
			var exCode = "create -n=TestContainer".Run();
			Assert.AreEqual(0, exCode);

			exCode = "remove -n=TestContainer".Run();
			Assert.AreEqual(0, exCode);
			
			exCode = "export -n=TestContainer -f=testKey.xml".Run();
			Assert.AreEqual(1, exCode);
		}
	}
}

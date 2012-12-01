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
			"remove".FailRun();
		}

		[Test]
		public void FromContainer()
		{
			"create -n=TestContainer".SuccessRun();

			"remove -n=TestContainer".SuccessRun();

			"export -n=TestContainer -f=testKey.xml".FailRun();
		}
	}
}

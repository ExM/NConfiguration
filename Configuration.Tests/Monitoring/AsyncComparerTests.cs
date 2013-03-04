using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Threading;

namespace Configuration.Monitoring
{
	[TestFixture]
	public class AsyncComparerTests
	{

		[Test]
		public void EmptyCompare()
		{
			string file = Path.GetTempFileName();
			File.WriteAllBytes(file, new byte[0]);

			bool result = SyncCompare(file, new byte[0]);

			Assert.That(result, Is.EqualTo(true));
		}

		[Test]
		public void NoFile()
		{
			string file = Path.GetRandomFileName();

			bool result = SyncCompare(file, new byte[0]);

			Assert.That(result, Is.EqualTo(false));
		}

		[TestCase(5, -1)]
		[TestCase(5, 0)]
		[TestCase(5, 2)]
		[TestCase(5, 4)]
		[TestCase(0x4200, -1)]
		[TestCase(0x4200, 0)]
		[TestCase(0x4200, 0x2000)]
		[TestCase(0x4200, 0x4000 - 2)]
		[TestCase(0x4200, 0x4000 - 1)]
		[TestCase(0x4200, 0x4000)]
		[TestCase(0x4200, 0x4000 + 1)]
		[TestCase(0x4200, 0x4000 + 2)]
		[TestCase(0x4200, 0x4200 - 2)]
		[TestCase(0x4200, 0x4200 - 1)]
		public void Compare(int totalBytes, int noEqPos)
		{
			StringBuilder sb = new StringBuilder(totalBytes);
			for(int i=0; i<totalBytes; i++)
				sb.Append('A');

			string content = sb.ToString();

			if(noEqPos != -1)
				sb[noEqPos] = 'B';

			string expected = sb.ToString();

			string file = Path.GetTempFileName();
			File.WriteAllBytes(file, Encoding.ASCII.GetBytes(content));

			bool result = SyncCompare(file, Encoding.ASCII.GetBytes(expected));

			Assert.That(result, Is.EqualTo(content == expected));
		}

		private static bool SyncCompare(string file, byte[] expected)
		{
			var wait = new ManualResetEvent(false);
			bool result = false;
			AsyncComparer.Compare(file, expected, eq =>
			{
				result = eq;
				wait.Set();
			});

			Assert.IsTrue(wait.WaitOne(10000), "10 sec elapsed");

			return result;
		}
	}
}
